#!/usr/bin/env python3
"""
extract_energy.py — Enrich raw run JSON files with energy metrics.

Input:  data/runs/<timestamp>/raw (contains run_*.json and run_*.perfetto-trace)
Output: data/runs/<timestamp>/raw/energy_metrics.csv

Method: integrate |current_ua| * voltage_uv over time and convert to mJ.
Fallback/secondary metric: derive a relative CPU energy proxy from Perfetto
CPU frequency residency when direct battery-current energy is unreliable.
"""

import argparse
import json
import os
import sys
from pathlib import Path

import numpy as np
import pandas as pd

try:
    from perfetto.trace_processor import TraceProcessor
    PERFETTO_LIB = True
except ImportError:
    PERFETTO_LIB = False
    print("[extract_energy] WARNING: 'perfetto' library not found.")
    print("        Install with: pip install perfetto")
    print("        Without it, energy cannot be extracted from traces.")


def extract_energy(trace_path: str) -> dict:
    result = {
        "trace_path": trace_path,
        "battery_csv_path": str(Path(trace_path).with_suffix(".battery.csv")),
        "energy_total_mj": None,
        "battery_energy_valid": False,
        "battery_current_nonzero_samples": None,
        "battery_current_nonzero_ratio": None,
        "avg_current_ma": None,
        "avg_voltage_v": None,
        "duration_s": None,
        "sample_count": None,
        "avg_power_mw": None,
        "avg_cpu_freq_mhz": None,
        "cpu_big_core_pct": None,
        "cpu_freq_residency_ghz_s": None,
        "cpu_energy_proxy_score": None,
        "cpu_energy_proxy_per_s": None,
        "peak_memory_rss_kb": None,
        "frame_janks": None,
        "energy_method": "battery_current_voltage_integration",
        "parse_error": None,
    }

    if not os.path.exists(trace_path):
        result["parse_error"] = "file not found"
        return result

    file_size = os.path.getsize(trace_path)
    if file_size < 2048:
        result["parse_error"] = f"trace too small ({file_size} bytes) — Perfetto likely didn't capture"
        return result

    if not PERFETTO_LIB:
        # Continue below with CSV fallback if available.
        result["parse_error"] = "perfetto library not installed"

    def apply_battery_csv_fallback(reason_prefix: str = ""):
        csv_path = Path(trace_path).with_suffix(".battery.csv")
        if not csv_path.exists():
            if reason_prefix:
                result["parse_error"] = f"{reason_prefix}; no battery csv fallback"
            return

        try:
            df = pd.read_csv(csv_path)
        except Exception as exc:
            if reason_prefix:
                result["parse_error"] = f"{reason_prefix}; battery csv unreadable: {exc}"
            else:
                result["parse_error"] = f"battery csv unreadable: {exc}"
            return

        for col in ["timestamp_s", "current_ua", "voltage_uv"]:
            if col not in df.columns:
                if reason_prefix:
                    result["parse_error"] = f"{reason_prefix}; battery csv missing '{col}'"
                else:
                    result["parse_error"] = f"battery csv missing '{col}'"
                return

        df["timestamp_s"] = pd.to_numeric(df["timestamp_s"], errors="coerce")
        df["current_ua"] = pd.to_numeric(df["current_ua"], errors="coerce")
        df["voltage_uv"] = pd.to_numeric(df["voltage_uv"], errors="coerce")

        df = df.dropna(subset=["timestamp_s", "current_ua", "voltage_uv"]).sort_values("timestamp_s")
        if len(df) < 2:
            if reason_prefix:
                result["parse_error"] = f"{reason_prefix}; insufficient battery csv samples"
            else:
                result["parse_error"] = "insufficient battery csv samples"
            return

        nonzero_mask = df["current_ua"] != 0
        nonzero_samples = int(nonzero_mask.sum())
        nonzero_ratio = float(nonzero_samples / len(df))
        result["battery_current_nonzero_samples"] = nonzero_samples
        result["battery_current_nonzero_ratio"] = round(nonzero_ratio, 3)
        result["battery_energy_valid"] = nonzero_samples >= 2 and nonzero_ratio >= 0.25

        df["current_ua"] = df["current_ua"].abs()
        df["power_mw"] = (df["current_ua"] * df["voltage_uv"]) / 1e9

        ts_s = df["timestamp_s"].values
        dt_s = np.diff(ts_s)
        power_mid = (df["power_mw"].values[:-1] + df["power_mw"].values[1:]) / 2
        energy_mj = float(np.sum(power_mid * dt_s))
        duration_s = float(ts_s[-1] - ts_s[0])

        result["energy_total_mj"] = round(energy_mj, 2)
        result["avg_current_ma"] = round(float(df["current_ua"].mean() / 1000), 2)
        result["avg_voltage_v"] = round(float(df["voltage_uv"].mean() / 1e6), 3)
        result["duration_s"] = round(duration_s, 1)
        result["sample_count"] = int(len(df))
        result["avg_power_mw"] = round(energy_mj / duration_s, 1) if duration_s > 0 else None
        result["energy_method"] = "adb_dumpsys_battery_polling"
        result["parse_error"] = None

    if not PERFETTO_LIB:
        apply_battery_csv_fallback("perfetto library not installed")
        return result

    try:
        tp = TraceProcessor(trace=trace_path)

        # ── Battery current + voltage ─────────────────────────────────────────
        batt_df = tp.query("""
            SELECT
                t.name  AS counter_name,
                c.ts    AS ts_ns,
                c.value AS value
            FROM counter c
            JOIN counter_track t ON c.track_id = t.id
            WHERE t.name IN (
                'batt.current_ua',
                'batt.voltage_uv',
                'android.power.battery_current_ua',
                'android.power.battery_voltage_uv'
            )
            ORDER BY t.name, c.ts
        """).as_pandas_dataframe()

        if batt_df.empty:
            result["parse_error"] = "no battery counter data in trace"
            apply_battery_csv_fallback("no battery counter data in trace")
        else:
            curr_df = batt_df[batt_df["counter_name"].str.contains("current")].copy()
            volt_df = batt_df[batt_df["counter_name"].str.contains("voltage")].copy()

            if not curr_df.empty and not volt_df.empty:
                curr_df = curr_df.sort_values("ts_ns").reset_index(drop=True)
                volt_df = volt_df.sort_values("ts_ns").reset_index(drop=True)

                # Align current and voltage samples by nearest timestamp (±1s tolerance)
                merged = pd.merge_asof(
                    curr_df[["ts_ns", "value"]].rename(columns={"value": "current_ua"}),
                    volt_df[["ts_ns", "value"]].rename(columns={"value": "voltage_uv"}),
                    on="ts_ns",
                    direction="nearest",
                    tolerance=1_000_000_000,  # 1 second in nanoseconds
                ).dropna()

                merged["current_ua"] = merged["current_ua"].abs()

                if len(merged) >= 2:
                    # Trapezoidal integration
                    # Power in µW = µA × µV / 1e6 → µW. Wait:
                    # µA × µV = µA × µV = 10^-6 A × 10^-6 V = 10^-12 W = pW
                    # To get mW: pW / 1e9 = mW
                    merged["power_mw"] = (merged["current_ua"] * merged["voltage_uv"]) / 1e9

                    ts_s = merged["ts_ns"].values / 1e9
                    dt_s = np.diff(ts_s)
                    power_mid = (merged["power_mw"].values[:-1] + merged["power_mw"].values[1:]) / 2
                    energy_mj = float(np.sum(power_mid * dt_s))

                    duration_s = ts_s[-1] - ts_s[0]

                    result["energy_total_mj"]  = round(energy_mj, 2)
                    result["avg_current_ma"]   = round(float(merged["current_ua"].mean() / 1000), 2)
                    result["avg_voltage_v"]    = round(float(merged["voltage_uv"].mean() / 1e6), 3)
                    result["duration_s"]       = round(duration_s, 1)
                    result["sample_count"]     = len(merged)

                    # Average power (useful sanity check: A53 web browsing ≈ 200–600mW)
                    result["avg_power_mw"] = round(energy_mj / duration_s, 1) if duration_s > 0 else None

        # ── CPU frequency (Exynos 1280: cores 0-1 big A78, 2-7 little A55) ───
        cpu_df = tp.query("""
            SELECT
                t.cpu,
                c.value AS freq_khz,
                c.ts,
                LEAD(c.ts) OVER (PARTITION BY t.cpu ORDER BY c.ts) - c.ts AS dur_ns
            FROM counter c
            JOIN cpu_counter_track t ON c.track_id = t.id
            WHERE t.name = 'cpufreq'
        """).as_pandas_dataframe()

        if not cpu_df.empty:
            cpu_df = cpu_df.dropna(subset=["dur_ns"])
            if not cpu_df.empty:
                total_cpu_time = cpu_df["dur_ns"].sum()
                weighted_freq = (cpu_df["freq_khz"] * cpu_df["dur_ns"]).sum() / total_cpu_time
                result["avg_cpu_freq_mhz"] = round(weighted_freq / 1000, 1)

                # Big core utilisation (cores 0-1 on Exynos 1280 are A78)
                big_time = cpu_df[cpu_df["cpu"].isin([0, 1])]["dur_ns"].sum()
                result["cpu_big_core_pct"] = round(100 * big_time / total_cpu_time, 1)

                cpu_df["dur_s"] = cpu_df["dur_ns"] / 1e9
                cpu_df["freq_ghz"] = cpu_df["freq_khz"] / 1e6
                cpu_df["cluster_weight"] = np.where(cpu_df["cpu"].isin([0, 1]), 2.0, 1.0)
                residency = float((cpu_df["freq_ghz"] * cpu_df["dur_s"]).sum())
                proxy_score = float(
                    (cpu_df["freq_ghz"] * cpu_df["dur_s"] * cpu_df["cluster_weight"]).sum()
                )
                trace_span_s = float(cpu_df["dur_s"].sum() / cpu_df["cpu"].nunique()) if cpu_df["cpu"].nunique() else None

                result["cpu_freq_residency_ghz_s"] = round(residency, 2)
                result["cpu_energy_proxy_score"] = round(proxy_score, 2)
                if trace_span_s and trace_span_s > 0:
                    result["cpu_energy_proxy_per_s"] = round(proxy_score / trace_span_s, 3)

        # ── Peak memory (Chrome renderer) ─────────────────────────────────────
        try:
            mem_df = tp.query("""
                SELECT MAX(anon_rss_and_swap_kb) AS peak_kb
                FROM memory_rss_and_swap_per_process_sample
                WHERE process_name LIKE '%chrome%' OR process_name LIKE '%chromium%'
            """).as_pandas_dataframe()
            if not mem_df.empty and mem_df["peak_kb"].iloc[0]:
                result["peak_memory_rss_kb"] = int(mem_df["peak_kb"].iloc[0])
        except Exception:
            pass

        # ── Frame janks ───────────────────────────────────────────────────────
        try:
            jank_df = tp.query("""
                SELECT COUNT(*) AS n FROM actual_frame_timeline_slice
                WHERE jank_type IS NOT NULL AND jank_type != 'None'
            """).as_pandas_dataframe()
            if not jank_df.empty:
                result["frame_janks"] = int(jank_df["n"].iloc[0])
        except Exception:
            pass

        tp.close()

    except Exception as e:
        result["parse_error"] = str(e)
        apply_battery_csv_fallback("perfetto parse failed")

    return result


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--raw-dir", help="Path to a run raw directory (preferred).")
    parser.add_argument("--results-dir", help="Deprecated alias of --raw-dir.")
    args = parser.parse_args()

    raw_dir_arg = args.raw_dir or args.results_dir
    if not raw_dir_arg:
        parser.error("one of --raw-dir or --results-dir is required")

    raw_dir = Path(raw_dir_arg)
    if args.results_dir and not args.raw_dir:
        print("[extract_energy] INFO: --results-dir is deprecated; use --raw-dir.")

    json_files = sorted(raw_dir.glob("run_*.json"))

    if not json_files:
        print(f"[extract_energy] No run_*.json files in {raw_dir}")
        sys.exit(1)

    print(f"[extract_energy] Processing {len(json_files)} runs...")
    rows = []

    for jf in json_files:
        trace_file = jf.with_suffix(".perfetto-trace")
        with open(jf) as f:
            run_data = json.load(f)

        energy = extract_energy(str(trace_file))
        run_data["energy"] = energy

        with open(jf, "w") as f:
            json.dump(run_data, f, indent=2)

        status = f"  {jf.name}: "
        if energy.get("energy_total_mj") is not None:
            status += (f"E={energy['energy_total_mj']}mJ "
                      f"P={energy.get('avg_power_mw')}mW "
                      f"t={energy.get('duration_s')}s "
                      f"n={energy.get('sample_count')}")
        else:
            status += f"ERROR: {energy.get('parse_error')}"
        print(status)

        rows.append({
            "run":       run_data["meta"]["run"],
            "condition": run_data["meta"]["condition"],
            "scenario":  run_data["meta"].get("scenario", "default"),
            **energy,
        })

    df = pd.DataFrame(rows)
    csv_path = raw_dir / "energy_metrics.csv"
    df.to_csv(csv_path, index=False)

    print(f"\n[extract_energy] Energy CSV: {csv_path}")
    print("\nSummary by condition/scenario:")
    valid = df[df["energy_total_mj"].notna()]
    if not valid.empty:
        print(valid.groupby(["condition", "scenario"])[["energy_total_mj", "avg_power_mw", "avg_current_ma"]].describe().round(2))
    else:
        print("  No valid energy data — check trace files and perfetto library installation")


if __name__ == "__main__":
    main()

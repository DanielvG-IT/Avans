#!/usr/bin/env python3
"""
analyze_runs.py — Generate statistics, report, and plots from run raw data.

Input:  data/runs/<timestamp>/raw (run_*.json files with metrics)
Output: statistics.csv and report.md in the raw directory, plus plots in the
run analysis directory
"""

import argparse
import json
import sys
from pathlib import Path

import numpy as np
import pandas as pd
from scipy import stats
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt

plt.rcParams.update({
    "font.family": "serif", "font.size": 10,
    "axes.spines.top": False, "axes.spines.right": False,
    "figure.dpi": 150,
})


def load_data(raw_dir: Path) -> pd.DataFrame:
    rows = []
    for jf in sorted(raw_dir.glob("run_*.json")):
        with open(jf) as f:
            d = json.load(f)

        row = {
            "run":       d["meta"]["run"],
            "condition": d["meta"]["condition"],
            "scenario":  d["meta"].get("scenario", "default"),
            "protocol_mode": d["meta"].get("protocol_mode", "cdp"),
            "browser_metrics_available": d["meta"].get("browser_metrics_available", True),
        }

        # Energy
        e = d.get("energy", {})
        row["energy_total_mj"]  = e.get("energy_total_mj")
        row["battery_energy_valid"] = e.get("battery_energy_valid")
        row["battery_current_nonzero_ratio"] = e.get("battery_current_nonzero_ratio")
        row["avg_power_mw"]     = e.get("avg_power_mw")
        row["avg_cpu_freq_mhz"] = e.get("avg_cpu_freq_mhz")
        row["cpu_freq_residency_ghz_s"] = e.get("cpu_freq_residency_ghz_s")
        row["cpu_energy_proxy_score"] = e.get("cpu_energy_proxy_score")
        row["cpu_energy_proxy_per_s"] = e.get("cpu_energy_proxy_per_s")
        row["peak_memory_kb"]   = e.get("peak_memory_rss_kb")
        row["frame_janks"]      = e.get("frame_janks")

        # Browser metrics (aggregated from pages)
        pages = d.get("pages", [])
        metric_pages = [p.get("metrics") for p in pages if p.get("metrics")]
        if metric_pages:
            p0 = metric_pages[0]   # cold load page
            row["lcp_cold_ms"]   = p0.get("lcp")
            row["fcp_cold_ms"]   = p0.get("firstContentfulPaint")
            row["ttfb_cold_ms"]  = p0.get("ttfb")
            row["tbt_total_ms"]  = sum(m.get("tbt", 0) for m in metric_pages)
            row["cls_cold"]      = p0.get("cls")
            row["transfer_kb"]   = sum(
                (m.get("transferSize") or 0) for m in metric_pages
            ) / 1024
            if p0.get("jsHeap"):
                row["js_heap_mb"] = p0["jsHeap"].get("used_mb")

        rows.append(row)

    return pd.DataFrame(rows)


def mann_whitney(a: pd.Series, b: pd.Series, label: str) -> dict:
    a, b = a.dropna().values, b.dropna().values
    if len(a) < 3 or len(b) < 3:
        return {"metric": label, "note": "insufficient data"}

    stat, p = stats.mannwhitneyu(a, b, alternative="two-sided")
    r = 1 - 2 * stat / (len(a) * len(b))   # rank-biserial correlation
    mag = "negligible" if abs(r) < .1 else "small" if abs(r) < .3 else "medium" if abs(r) < .5 else "large"

    return {
        "metric": label,
        "n_ssr": len(a), "n_csr": len(b),
        "median_ssr": round(float(np.median(a)), 2),
        "iqr_ssr":    round(float(np.percentile(a, 75) - np.percentile(a, 25)), 2),
        "median_csr": round(float(np.median(b)), 2),
        "iqr_csr":    round(float(np.percentile(b, 75) - np.percentile(b, 25)), 2),
        "u_stat": round(float(stat), 1),
        "p_value": round(float(p), 5),
        "effect_r": round(float(r), 3),
        "effect_magnitude": mag,
        "direction": "SSR lower" if np.median(a) < np.median(b) else "CSR lower",
    }


def boxplot(ssr: pd.Series, csr: pd.Series, title: str, unit: str, path: str):
    fig, ax = plt.subplots(figsize=(3.5, 4))
    data = [ssr.dropna().values, csr.dropna().values]
    bp = ax.boxplot(data, patch_artist=True, widths=0.45,
                    medianprops={"color": "black", "linewidth": 1.8})
    for patch, col in zip(bp["boxes"], ["#4E79A7", "#E15759"]):
        patch.set_facecolor(col); patch.set_alpha(0.65)
    for i, (vals, col) in enumerate(zip(data, ["#4E79A7", "#E15759"]), 1):
        jitter = np.random.uniform(-0.07, 0.07, len(vals))
        ax.scatter(np.full(len(vals), i) + jitter, vals, s=10, alpha=0.35, c=col, zorder=3)
    ax.set_xticks([1, 2]); ax.set_xticklabels(["SSR", "CSR"])
    ax.set_ylabel(f"{title} ({unit})"); ax.set_title(title, fontsize=10, pad=6)
    plt.tight_layout()
    plt.savefig(path, bbox_inches="tight"); plt.close()


def energy_lineplot(stats_df: pd.DataFrame, path: str):
    energy_df = stats_df[stats_df["metric"] == "Total energy"].copy()
    if energy_df.empty:
        return

    scenario_sizes = {
        "static": 72,
        "dynamic": 6000,
        "massive": 24000,
    }
    scenario_labels = {
        "static": "Static",
        "dynamic": "Dynamic",
        "massive": "Massive",
    }

    energy_df["dataset_size"] = energy_df["scenario"].map(scenario_sizes)
    energy_df = (
        energy_df.dropna(subset=["dataset_size"])
        .sort_values("dataset_size")
        .reset_index(drop=True)
    )
    if energy_df.empty:
        return

    x = energy_df["dataset_size"].astype(float).to_numpy()

    fig, ax = plt.subplots(figsize=(6.6, 4.1))
    ax.plot(
        x,
        energy_df["median_ssr"],
        color="#4E79A7",
        marker="o",
        markersize=6.5,
        linewidth=2.2,
        label="SSR",
    )
    ax.plot(
        x,
        energy_df["median_csr"],
        color="#E15759",
        marker="o",
        markersize=6.5,
        linewidth=2.2,
        label="CSR",
    )

    ax.set_xscale("log")
    ax.set_xticks(x)
    ax.set_xticklabels(
        [
            f"{scenario_labels[row.scenario]}\n{int(row.dataset_size):,}"
            for row in energy_df.itertuples()
        ]
    )
    ax.set_xlabel("Dataset size (records, log scale)")
    ax.set_ylabel("Median total energy (mJ)")
    ax.grid(axis="y", linestyle="--", linewidth=0.7, alpha=0.35)
    ax.legend(frameon=False, loc="upper left")
    plt.tight_layout()
    plt.savefig(path, bbox_inches="tight")
    plt.close()


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--raw-dir", help="Path to a run raw directory (preferred).")
    parser.add_argument("--results-dir", help="Deprecated alias of --raw-dir.")
    args = parser.parse_args()

    raw_dir_arg = args.raw_dir or args.results_dir
    if not raw_dir_arg:
        parser.error("one of --raw-dir or --results-dir is required")

    rd = Path(raw_dir_arg)
    run_dir = rd.parent if rd.name == "raw" else rd
    analysis_dir = run_dir / "analysis"
    analysis_dir.mkdir(exist_ok=True)
    if args.results_dir and not args.raw_dir:
        print("[analyze_runs] INFO: --results-dir is deprecated; use --raw-dir.")

    df = load_data(rd)
    if df.empty:
        print("[analyze_runs] No data."); sys.exit(1)

    scenarios = sorted(df["scenario"].dropna().unique())
    print(f"[analyze_runs] Loaded {len(df)} runs across scenarios: {', '.join(scenarios)}")
    battery_valid_runs = int(df["battery_energy_valid"].fillna(False).sum()) if "battery_energy_valid" in df.columns else 0
    print(f"[analyze_runs] battery-valid runs: {battery_valid_runs}/{len(df)}")
    protocol_counts = (
        df.groupby(["protocol_mode", "browser_metrics_available"])
          .size()
          .reset_index(name="count")
    )
    for _, record in protocol_counts.iterrows():
        print(
            "[analyze_runs] "
            f"protocol={record['protocol_mode']} "
            f"browser_metrics_available={record['browser_metrics_available']} "
            f"count={record['count']}"
        )

    metrics = [
        ("energy_total_mj",  "Total energy",              "mJ"),
        ("avg_power_mw",     "Average power",             "mW"),
        ("cpu_energy_proxy_score", "CPU energy proxy score", "score"),
        ("cpu_energy_proxy_per_s", "CPU energy proxy rate", "score/s"),
        ("cpu_freq_residency_ghz_s", "CPU frequency residency", "GHz*s"),
        ("lcp_cold_ms",      "LCP — cold load",           "ms"),
        ("fcp_cold_ms",      "FCP — cold load",           "ms"),
        ("ttfb_cold_ms",     "TTFB — cold load",          "ms"),
        ("tbt_total_ms",     "Total Blocking Time (all pages)", "ms"),
        ("cls_cold",         "CLS — cold load",           "score"),
        ("transfer_kb",      "Navigation response size",  "KB"),
        ("js_heap_mb",       "JS heap sample",            "MB"),
        ("peak_memory_kb",   "Peak RSS (Perfetto)",       "KB"),
        ("avg_cpu_freq_mhz", "Avg CPU frequency",         "MHz"),
        ("frame_janks",      "Frame janks",               "count"),
    ]

    plots_dir = analysis_dir / "plots"
    plots_dir.mkdir(exist_ok=True)
    results = []

    for scenario in scenarios:
        ssr = df[(df["condition"] == "ssr") & (df["scenario"] == scenario)]
        csr = df[(df["condition"] == "csr") & (df["scenario"] == scenario)]

        print(f"[analyze_runs] Scenario '{scenario}': {len(ssr)} SSR runs, {len(csr)} CSR runs")

        for col, label, unit in metrics:
            if col not in df.columns or df[col].dropna().empty:
                continue

            r = mann_whitney(ssr[col], csr[col], label)
            r["scenario"] = scenario
            results.append(r)

            if "p_value" in r:
                print(
                    f"  [{scenario}] {label:27s} SSR={r['median_ssr']:8.1f}  "
                    f"CSR={r['median_csr']:8.1f}  p={r['p_value']:.4f}  "
                    f"r={r['effect_r']:.3f} ({r['effect_magnitude']})"
                )

            try:
                boxplot(
                    ssr[col],
                    csr[col],
                    f"{label} ({scenario})",
                    unit,
                    str(plots_dir / f"{scenario}_{col}.png"),
                )
            except Exception:
                pass

    # ── Save stats ─────────────────────────────────────────────────────────────
    stats_df = pd.DataFrame(results)
    stats_df.to_csv(rd / "statistics.csv", index=False)

    try:
        energy_lineplot(
            stats_df,
            str(analysis_dir / "energy_by_dataset_size_line.png"),
        )
    except Exception:
        pass

    # ── Bonferroni-corrected significance threshold ────────────────────────────
    n_tests = len([r for r in results if "p_value" in r])
    alpha_b = 0.05 / n_tests if n_tests else 0.05

    # ── Markdown report ────────────────────────────────────────────────────────
    lines = [
        "# SSR vs CSR Benchmark — Results",
        "",
        f"**Device:** Samsung SM-A536B (Galaxy A53), Android 16, SDK 36  ",
        f"**Direct energy method:** Battery current × voltage integration (250ms polling)  ",
        f"**Proxy energy method:** Perfetto CPU frequency residency weighted by core class  ",
        f"**Statistical test:** Mann-Whitney U, two-sided, α = 0.05  ",
        f"**Bonferroni-corrected threshold:** α = {alpha_b:.4f} ({n_tests} tests)  ",
        "",
        "## Results table",
        "",
        "| Scenario | Metric | SSR median (IQR) | CSR median (IQR) | *p* | Effect *r* | Sig |",
        "|----------|--------|------------------|------------------|-----|-----------|-----|",
    ]
    for r in results:
        if "p_value" not in r: continue
        sig = "✓" if r["p_value"] < alpha_b else "–"
        pstr = f"{r['p_value']:.4f}" if r["p_value"] >= 0.0001 else "<0.0001"
        lines.append(
            f"| {r.get('scenario', 'default')} "
            f"| {r['metric']} "
            f"| {r['median_ssr']:.1f} (±{r['iqr_ssr']:.1f}) "
            f"| {r['median_csr']:.1f} (±{r['iqr_csr']:.1f}) "
            f"| {pstr} | {r['effect_r']:.3f} ({r['effect_magnitude']}) | {sig} |"
        )
    lines += [
        "",
        "## Methodology notes",
        "",
        "- All runs interleaved in randomised order to prevent thermal session bias.",
        "- Chrome force-stopped before each run; no app-data wipe between runs.",
        "- Screen brightness fixed at 120/255 (manual mode) throughout.",
        "- Device radios: WiFi only (mobile data, Bluetooth, NFC disabled).",
        "- Device idle ≥45s between runs; runs discarded if start temp >37°C.",
        "- Energy computed as ∫ |I(t)| × V(t) dt over trace window (trapezoidal rule).",
        "- Battery-current energy under USB-connected runs may be contaminated by external power and should be treated cautiously when many samples are zero-current.",
        "- CPU energy proxy is an inference from Perfetto CPU frequency residency, weighted more heavily for big cores; it supports relative SSR/CSR comparison, not absolute joule claims.",
        "- SM-A536B does not expose hardware power rail counters; battery polling used.",
        "- Runs without a working CDP connection remain usable for system-level trace and energy analysis, but browser-level metrics are excluded when unavailable.",
        "",
    ]
    (rd / "report.md").write_text("\n".join(lines))
    print(
        f"\n[analyze_runs] OK: statistics/report saved in {rd}; "
        f"plots saved in {analysis_dir}"
    )


if __name__ == "__main__":
    main()

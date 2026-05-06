# SSR vs CSR Energy Benchmark

### Samsung SM-A536B (Galaxy A53) — Android 16

One command runs everything. This README documents the exact steps.

---

## Quick start

```bash
# 1. Install dependencies
npm install
pip install -r requirements.txt

# 2. Start your apps (two terminals)
cd your-ssr-app && npm run start    # → port 3000
cd your-csr-app && npm run start    # → port 3001

# 3. Find your Mac's local IP
ipconfig getifaddr en0

# 4. Run the benchmark (takes ~3-4 hours for 30 runs each)
bash run.sh \
  --ssr-url http://YOUR_IP:3000 \
  --csr-url http://YOUR_IP:3001 \
  --runs 30 \
  --scenarios static,dynamic,massive \
  --ssr-pages '/ssr?scenario={scenario}' \
  --csr-pages '/csr?scenario={scenario}'
```

---

## Before running — one manual step

Chrome may show a "set as default browser" banner the first time it opens on a
fresh device setup. Dismiss it manually once, then it won't appear again
during the benchmark (Puppeteer dismisses dialogs automatically).

To trigger it early:

```bash
adb shell am start -a android.intent.action.VIEW -d "http://YOUR_IP:3000"
# dismiss the banner on the phone, then close Chrome
adb shell am force-stop com.android.chrome
```

---

## Configure scenario routes

The runner supports placeholder substitution in page paths:

- `{scenario}` is replaced with each value from `--scenarios`
- defaults are:
  - `--scenarios static,dynamic,massive`
  - `--ssr-pages '/ssr?scenario={scenario}'`
  - `--csr-pages '/csr?scenario={scenario}'`

Examples:

```bash
# one route per case
--ssr-pages '/ssr?scenario={scenario}'
--csr-pages '/csr?scenario={scenario}'

# multiple navigations per run
--ssr-pages '/ssr?scenario={scenario},/ssr?scenario={scenario}&q=audio'
--csr-pages '/csr?scenario={scenario},/csr?scenario={scenario}&q=audio'
```

Each scenario gets exactly `--runs` SSR and `--runs` CSR runs, then all cases are shuffled into one interleaved sequence.

### Why use `?scenario=` instead of separate pages?

Using query parameters keeps the benchmark fair and easier to maintain:

- Same page/component path for each mode, which reduces accidental implementation drift.
- Scenario changes stay data-level (`static`, `dynamic`, `massive`) instead of route-level code differences.
- Route templates in `run.sh` stay simple and deterministic with `{scenario}` substitution.
- Results are more defensible: differences are more likely due to SSR vs CSR, not page-specific code.

## Output

```text
data/runs/YYYYMMDD_HHMMSS/
├── raw/
│   ├── run_order.txt ← randomised condition sequence (reproducibility)
│   ├── temperatures.csv ← per-run start temperature log
│   ├── run_N_ssr.json ← DevTools metrics per run (LCP, FCP, TBT, etc.)
│   ├── run_N_ssr.perfetto-trace ← raw trace (open at ui.perfetto.dev)
│   └── run_N_ssr.battery.csv ← fallback battery telemetry samples
├── processed/
│   └── energy_metrics.csv ← extracted energy per run
└── analysis/
    ├── statistics.csv ← Mann-Whitney U results
    ├── report.md ← paper-ready results table
    └── plots/ ← box plots (energy, LCP, TBT, ...)
```

---

## Device notes (SM-A536B)

**No hardware power rails:** The A53 does not expose subsystem power counters
via PowerStats HAL. Energy is measured by integrating battery current (µA) ×
voltage (µV) at 250ms polling via Perfetto `android.power`. This is the same
method used in published mobile energy research. Document in your paper as:

> "Energy consumption was measured by numerically integrating instantaneous
> battery current (µA) and voltage (µV) samples collected at 250 ms intervals
> via Perfetto's android.power data source on a Samsung Galaxy A53 (SM-A536B,
> Android 16). Energy is reported in millijoules (mJ) computed using the
> trapezoidal rule over the measurement window."

**Exynos 1280 thermal behaviour:** The A53's Exynos 1280 reaches thermal
throttle faster than Snapdragon equivalents. The 45s cooldown and 37°C gate
are calibrated for this SoC. If you see many retry warnings, increase
`COOL_WAIT_S` to 60.

**Chrome DevTools on Samsung:** Puppeteer connects via ADB port forwarding
to Chrome's remote debugging port. This works on Samsung without root.
The `adb forward tcp:9222 localabstract:chrome_devtools_remote` command is
re-run before each scenario in case the ADB connection resets.

---

## Reproducing from raw data

Analysis only (no device needed):

```bash
python3 analysis/parse_perfetto.py --results-dir data/runs/YYYYMMDD_HHMMSS/raw
python3 analysis/analyse.py        --results-dir data/runs/YYYYMMDD_HHMMSS/raw

# preferred names
python3 analysis/extract_energy.py --results-dir data/runs/YYYYMMDD_HHMMSS/raw
python3 analysis/analyze_runs.py   --results-dir data/runs/YYYYMMDD_HHMMSS/raw
```

Publish the full `data/runs/YYYYMMDD_HHMMSS/` folder for full transparency.

# Analysis Scripts

These scripts work per benchmark batch on a single raw run folder:

- `data/runs/YYYYMMDD_HHMMSS/raw`

## What each script does

- `extract_energy.py`
  - Reads `run_*.json` and `run_*.perfetto-trace` from the raw folder.
  - Enriches each `run_*.json` with an `energy` section.
  - Writes `energy_metrics.csv` in the same raw folder.

- `analyze_runs.py`
  - Reads enriched `run_*.json` files from the raw folder.
  - Writes `statistics.csv`, `report.md`, and `plots/*.png` in the same raw folder.

## Preferred commands

```bash
python3 analysis/extract_energy.py --raw-dir data/runs/YYYYMMDD_HHMMSS/raw
python3 analysis/analyze_runs.py   --raw-dir data/runs/YYYYMMDD_HHMMSS/raw
```

## Legacy command compatibility

The old script names still work and forward to the new ones:

- `parse_perfetto.py` -> `extract_energy.py`
- `analyse.py` -> `analyze_runs.py`

The old argument `--results-dir` also still works, but `--raw-dir` is preferred.

# Research Project Repository

This repository contains the full SSR vs CSR performance and energy study: experiment scripts, run-batch data, analysis code, and paper assets.

## Repository Structure

- `experiment/`: Measurement runner (`run.sh`), Perfetto config, Puppeteer scenario scripts, and benchmark dependencies.
- `prototype/`: Next.js SSR/CSR prototype app.
- `data/runs/`: One folder per benchmark batch (`YYYYMMDD_HHMMSS/`) with `raw/`, `processed/`, and `analysis/` outputs together.
- `analysis/`: Python analysis scripts (`extract_energy.py`, `analyze_runs.py`) and compatibility wrappers.
- `paper/`: LaTeX manuscript and references.
- `docs/`: Proposal and supporting notes.

## Quick Start

```bash
# prototype app
cd prototype
npm install
npm run dev

# benchmark runner
cd ../experiment
npm install
pip install -r requirements.txt
bash run.sh --ssr-url http://YOUR_IP:3000 --csr-url http://YOUR_IP:3001 --runs 5
```

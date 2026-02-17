# L2S1LU2-Recommendation

This repository contains a student recommendation system project developed for a course assignment. The project explores building and evaluating recommendation models using a dataset of student information and skills, with experiments implemented in Jupyter notebooks.

## Contents

- `Data/`
  - `Raw/` — original dataset(s) (e.g. `Uitgebreide_VKM_dataset.csv`)
  - `Cleaned/` — cleaned CSVs ready for modeling (`cleaned_dataset.csv`, `cleaned_dataset_hard-NLP.csv`, `cleaned_dataset_soft-NLP.csv`)
- `Notebooks/` — analysis, preprocessing, training and evaluation notebooks
  - `1_data_exploration.ipynb` — EDA and initial inspection
  - `2_data_cleaning.ipynb` — data cleaning pipeline and outputs
  - `3.1_model_training_bow.ipynb` — BOW model training
  - `3.2_model_training_se.ipynb` — semantic / sentence-embedding model training
  - `4.1_evaluation_bow.ipynb` — evaluation for BOW models
  - `4.2_evaluation_se.ipynb` — evaluation for semantic models
  - `5.1_optimization_bow.ipynb` — hyperparameter tuning BOW
  - `5.2_optimization_se.ipynb` — hyperparameter tuning semantic models
  - `6_comparison.ipynb` — compare approaches and results
- `Notebooks/helpers/` — helper modules used by notebooks (`functs`, `notebook_pipelines`)
- `Schemas/` — design and diagrams (e.g. `ModelDeployment.drawio`)

## Project Overview

The goal is to build a recommender that maps student profiles and skill descriptions to suitable recommendations (e.g., study tracks, modules, or projects). The repository contains preprocessing code, multiple modeling approaches (bag-of-words and semantic embeddings), evaluation experiments, and optimization notebooks to tune models.

## Quickstart

1. Create a Python environment and install dependencies:

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```

2. Open the notebooks with Jupyter Lab/Notebook:

```bash
jupyter lab
# or
jupyter notebook
```

3. Run the notebooks in order starting from `Notebooks/1_data_exploration.ipynb` through `6_comparison.ipynb` to reproduce the analysis and results.

Notes:

- The `requirements.txt` file lists the Python packages used. If you only want to run specific notebooks, ensure the corresponding helper modules in `Notebooks/helpers/` are importable (append the repo root to `sys.path` or install the package in editable mode).
- Large data files are stored in `Data/Raw/` and `Data/Cleaned/`. Use the cleaned CSVs for training to avoid re-running expensive preprocessing.

## Notebooks and Code Layout

- Preprocessing and mapping utilities: `Notebooks/helpers/functs/*.py` (e.g., `NLP.py`, `StudentProfile.py`, `nlp_backmap.py`).
- Notebook pipelines that encapsulate model training workflows are under `Notebooks/helpers/notebook_pipelines/`.

## Reproducing results

- Run the cleaning pipeline in `2_data_cleaning.ipynb` to generate cleaned CSVs.
- Use `3.1` / `3.2` notebooks to train baseline models. Save model checkpoints and outputs in the notebook or export them to a `models/` folder if you add one.
- Run the corresponding evaluation notebooks in `4.*` to compute metrics and generate comparison figures.

## License

This repository includes a `LICENSE` file at the project root. Refer to it for license details.

# CompassGPT AI

This is the AI (machine learning) component of the CompassGPT project. It provides data processing, model training, and prediction services for the application.

## Features

- Data cleaning and preprocessing
- Model training and inference
- REST API for predictions

## Requirements

- Python 3.10+
- pip
- (Optional) Docker

## Installation

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd AI
   ```
2. (Recommended) Create a virtual environment:
   ```bash
   python3 -m venv venv
   source venv/bin/activate
   ```
3. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```

## Running Locally

### Start the API

```bash
python main.py
```

The API will start on the default port (see `main.py`).

### Using Docker

Build and run the container:

```bash
docker build -t compassgpt-ai .
docker run -p 8000:8000 compassgpt-ai
```

## Data

- Place your datasets in the `data/` directory as needed.

## Configuration

- Adjust paths and parameters in `main.py` or the relevant service/controller files as needed.

## Project Structure

- `controllers/` – API endpoints
- `services/` – Business logic and ML
- `models/` – Data models
- `data/` – Datasets

## License

See [LICENSE](LICENSE).

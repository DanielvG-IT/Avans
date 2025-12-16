# CompassGPT — AI

This folder contains the small AI service used by CompassGPT (Python).

Requirements

- Python 3.10+ (use a virtualenv)
- Docker (optional)

Quick start (local)

1. Create and activate a virtual environment:

   python -m venv .venv
   source .venv/bin/activate

2. Install dependencies:

   pip install -r requirements.txt

3. Run the service:

   python main.py

Docker

- Build image:

  docker build -t compassgpt-ai:latest .

- Run container (pass needed env vars):

  docker run --rm -p 8000:8000 compassgpt-ai:latest

Notes

- See `requirements.txt` for Python dependencies and `Dockerfile` for container setup.

License

- See repository `LICENSE`.

# AI

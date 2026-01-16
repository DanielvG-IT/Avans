# Use Python slim image
FROM python:3.11-slim

WORKDIR /app

# Ensure application root is on Python path
ENV PYTHONPATH=/app \
    PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1

# 1. Install system dependencies (needed for scikit-learn/scipy builds)
RUN apt-get update && apt-get install -y --no-install-recommends \
    gcc \
    g++ \
    && rm -rf /var/lib/apt/lists/*

# 2. OPTIMIZATION: Install heavy AI libraries FIRST and force CPU versions
# This layer is huge but changes rarely, so it will stay cached.
RUN pip install --no-cache-dir --upgrade pip && \
    pip install --no-cache-dir \
    torch --index-url https://download.pytorch.org/whl/cpu && \
    pip install --no-cache-dir sentence-transformers scikit-learn

# 3. Copy requirements (which should now only contain light web libraries)
COPY requirements.txt ./

# 4. Install remaining dependencies
# Because torch/sentence-transformers are already installed, pip will skip them here.
RUN pip install --no-cache-dir -r requirements.txt

# 5. Copy application code
COPY . .

# 6. Security: Create non-root user
RUN useradd -m -u 1001 aiuser && \
    chown -R aiuser:aiuser /app
USER aiuser

EXPOSE 8000

# Health check (simple python http request)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD python -c "import urllib.request; urllib.request.urlopen('http://localhost:8000/health', timeout=3)" || exit 1

# Use a small boot script that ensures /app is on sys.path before importing app
CMD ["python", "boot.py"]
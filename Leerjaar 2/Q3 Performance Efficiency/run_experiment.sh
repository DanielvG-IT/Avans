#!/usr/bin/env bash
set -euo pipefail

# Helper script to make experiment runs repeatable and easy from any cwd.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROTOTYPE_DIR="$SCRIPT_DIR/prototype"
EXPERIMENT_SCRIPT="$SCRIPT_DIR/experiment/experiment.sh"
NEXT_PID=""
APP_PORT="${APP_PORT:-3000}"
RUNS="${RUNS:-10}"
SCENARIOS="${SCENARIOS:-static,dynamic,massive}"
COOL_WAIT_S="${COOL_WAIT_S:-45}"
BATTERY_POLL_S="${BATTERY_POLL_S:-0.25}"
DEVICE_ID="${DEVICE_ID:-}"

cleanup() {
  if [[ -n "${NEXT_PID:-}" ]]; then
    kill "$NEXT_PID" >/dev/null 2>&1 || true
    wait "$NEXT_PID" 2>/dev/null || true
  fi
}

trap cleanup EXIT

wait_for_server() {
  local url="$1"
  local attempts=30

  for ((i=1; i<=attempts; i++)); do
    if curl -fsS "$url" >/dev/null 2>&1; then
      return 0
    fi
    sleep 1
  done

  echo "ERROR: Timed out waiting for Next.js server at $url"
  return 1
}

ensure_port_available() {
  if lsof -nP -iTCP:"$APP_PORT" -sTCP:LISTEN >/dev/null 2>&1; then
    echo "ERROR: Port ${APP_PORT} is already in use."
    echo "Stop the existing server or run with a different port, e.g. APP_PORT=3001 ./run_experiment.sh"
    exit 1
  fi
}

# Build and start the Next.js prototype (SSR/CSR) in the background.
ensure_port_available
cd "$PROTOTYPE_DIR"
npm run build
PORT="$APP_PORT" npm run start &
NEXT_PID=$!
cd "$SCRIPT_DIR"

sleep 1
if ! kill -0 "$NEXT_PID" >/dev/null 2>&1; then
  echo "ERROR: Next.js server exited before the experiment started."
  exit 1
fi

wait_for_server "http://127.0.0.1:${APP_PORT}"

ADB_ARGS=()
EXPERIMENT_ARGS=()
if [[ -n "$DEVICE_ID" ]]; then
  ADB_ARGS=(-s "$DEVICE_ID")
  EXPERIMENT_ARGS+=(--device-id "$DEVICE_ID")
fi

# Tunnel localhost port to Android device.
adb "${ADB_ARGS[@]+${ADB_ARGS[@]}}" reverse "tcp:${APP_PORT}" "tcp:${APP_PORT}"

# Run the main experiment script with localhost URLs.
bash "$EXPERIMENT_SCRIPT" \
  --ssr-url "http://localhost:${APP_PORT}" \
  --csr-url "http://localhost:${APP_PORT}" \
  --runs "$RUNS" \
  --scenarios "$SCENARIOS" \
  --cool-wait-s "$COOL_WAIT_S" \
  --battery-poll-s "$BATTERY_POLL_S" \
  "${EXPERIMENT_ARGS[@]+${EXPERIMENT_ARGS[@]}}" \
  --ssr-pages '/ssr?scenario={scenario}' \
  --csr-pages '/csr?scenario={scenario}'

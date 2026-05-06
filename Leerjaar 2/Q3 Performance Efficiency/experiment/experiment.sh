#!/usr/bin/env bash
set -euo pipefail

# =============================================================================
# SSR vs CSR energy benchmark for Samsung SM-A536B (Galaxy A53)
#
# Usage:
#   bash experiment.sh --ssr-url http://localhost:3000 \
#                      --csr-url http://localhost:3000 \
#                      --runs 30 \
#                      --scenarios static,dynamic,massive \
#                      --ssr-pages '/ssr?scenario={scenario}' \
#                      --csr-pages '/csr?scenario={scenario}'
#
# Prerequisites (one-time, on the phone):
#   1. chrome://flags/#enable-command-line-on-non-rooted-devices → Enabled
#   2. USB debugging enabled, device authorised
#   3. adb reverse already handled — this script sets it each run
# =============================================================================

# ── Config ───────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
RUNS=30
SSR_URL=""
CSR_URL=""
SCENARIOS="static,dynamic,massive"
SSR_PAGES="/ssr?scenario={scenario}"
CSR_PAGES="/csr?scenario={scenario}"
MAX_TEMP_C=37
COOL_WAIT_S=45
BATTERY_POLL_S=0.25
RUN_ID="$(date +%Y%m%d_%H%M%S)"
RUN_ROOT_DIR="${REPO_ROOT}/data/runs/${RUN_ID}"
RAW_DIR="${RUN_ROOT_DIR}/raw"
PROCESSED_DIR="${RUN_ROOT_DIR}/processed"
ANALYSIS_DIR="${RUN_ROOT_DIR}/analysis"
DEVICE_ID="${DEVICE_ID:-}"
BATTERY_POLL_PID=""
PERFETTO_CONFIG="${SCRIPT_DIR}/perfetto_config.pbtxt"
PUPPETEER_SCENARIO="${SCRIPT_DIR}/scripts/puppeteer-scenario.js"
EXTRACT_ENERGY_SCRIPT="${REPO_ROOT}/analysis/extract_energy.py"
ANALYZE_RUNS_SCRIPT="${REPO_ROOT}/analysis/analyze_runs.py"

echo "=== SSR vs CSR Energy Benchmark ==="

# ── Args ─────────────────────────────────────────────────────────
while [[ $# -gt 0 ]]; do
  case $1 in
    --ssr-url)    SSR_URL="$2";    shift 2 ;;
    --csr-url)    CSR_URL="$2";    shift 2 ;;
    --runs)       RUNS="$2";       shift 2 ;;
    --scenarios)  SCENARIOS="$2";  shift 2 ;;
    --ssr-pages)  SSR_PAGES="$2";  shift 2 ;;
    --csr-pages)  CSR_PAGES="$2";  shift 2 ;;
    --device-id)  DEVICE_ID="$2";  shift 2 ;;
    --cool-wait-s) COOL_WAIT_S="$2"; shift 2 ;;
    --battery-poll-s) BATTERY_POLL_S="$2"; shift 2 ;;
    *) echo "Unknown argument: $1"; exit 1 ;;
  esac
done

[[ -z "$SSR_URL" || -z "$CSR_URL" ]] && {
  echo "ERROR: --ssr-url and --csr-url are required"
  exit 1
}

# ── Preflight checks ─────────────────────────────────────────────
[[ ! -f "$PERFETTO_CONFIG" ]] && {
  echo "ERROR: perfetto_config.pbtxt not found at $PERFETTO_CONFIG"
  exit 1
}

[[ ! -f "$PUPPETEER_SCENARIO" ]] && {
  echo "ERROR: scripts/puppeteer-scenario.js not found at $PUPPETEER_SCENARIO"
  exit 1
}

[[ ! -f "$EXTRACT_ENERGY_SCRIPT" || ! -f "$ANALYZE_RUNS_SCRIPT" ]] && {
  echo "ERROR: analysis scripts not found under $REPO_ROOT/analysis"
  exit 1
}

# ── Device detection ─────────────────────────────────────────────
if [[ -z "$DEVICE_ID" ]]; then
  DEVICE_LINES=$(adb devices | awk 'NR>1 && $2=="device" {print $1}')
  DEVICE_COUNT=$(echo "$DEVICE_LINES" | sed '/^$/d' | wc -l | tr -d ' ')

  if [[ "$DEVICE_COUNT" -eq 1 ]]; then
    DEVICE_ID=$(echo "$DEVICE_LINES" | head -n 1)
  elif [[ "$DEVICE_COUNT" -eq 0 ]]; then
    echo "ERROR: no connected Android device found."
    echo "Tip: connect a device and run 'adb devices', or pass --device-id <serial>."
    exit 1
  else
    echo "ERROR: multiple devices connected; specify --device-id <serial>."
    echo "Connected devices:"
    echo "$DEVICE_LINES"
    exit 1
  fi
fi

echo "Using device: $DEVICE_ID"

mkdir -p "$RAW_DIR" "$PROCESSED_DIR" "$ANALYSIS_DIR"

# ── Battery poller ───────────────────────────────────────────────
start_battery_poller() {
  local out_csv="$1"
  echo "timestamp_s,current_ua,voltage_uv,temp_deci_c,charge_counter_uah,source" > "$out_csv"

  (
    while true; do
      local ts_s
      ts_s=$(python3 -c 'import time; print(f"{time.time():.3f}")' 2>/dev/null || date +%s)

      local dump
      dump=$(adb -s "$DEVICE_ID" shell dumpsys battery | tr -d '\r')

      local current_ua voltage_mv voltage_uv temp_deci_c charge_counter_uah

      current_ua=$(echo "$dump"       | awk -F': *' '/^  current now:/{print $2; exit}')
      voltage_mv=$(echo "$dump"       | awk -F': *' '/^  voltage:/{print $2; exit}')
      temp_deci_c=$(echo "$dump"      | awk -F': *' '/^  temperature:/{print $2; exit}')
      charge_counter_uah=$(echo "$dump" | awk -F': *' '/^  charge counter:/{print $2; exit}')

      # Samsung dumpsys often reports current in mA — normalise to uA
      if [[ "$current_ua" =~ ^-?[0-9]+$ ]] && (( current_ua > -20000 && current_ua < 20000 )); then
        current_ua=$((current_ua * 1000))
      fi

      if [[ "$voltage_mv" =~ ^[0-9-]+$ ]]; then
        voltage_uv=$((voltage_mv * 1000))
      else
        voltage_uv=""
      fi

      echo "$ts_s,${current_ua:-},${voltage_uv:-},${temp_deci_c:-},${charge_counter_uah:-},dumpsys_battery" >> "$out_csv"
      sleep "$BATTERY_POLL_S"
    done
  ) </dev/null &

  BATTERY_POLL_PID=$!
}

stop_battery_poller() {
  if [[ -n "${BATTERY_POLL_PID:-}" ]]; then
    kill "$BATTERY_POLL_PID" >/dev/null 2>&1 || true
    wait "$BATTERY_POLL_PID" 2>/dev/null || true
    BATTERY_POLL_PID=""
  fi
}

summarize_battery_quality() {
  local battery_csv="$1"
  python3 - <<'PY' "$battery_csv"
import csv
import sys

path = sys.argv[1]
rows = 0
nonzero = 0

with open(path, newline="") as f:
    reader = csv.DictReader(f)
    for row in reader:
        rows += 1
        try:
            current = float(row.get("current_ua") or 0)
        except ValueError:
            current = 0
        if current != 0:
            nonzero += 1

print(f"{rows},{nonzero}")
PY
}

trap stop_battery_poller EXIT

write_chrome_command_line() {
  adb -s "$DEVICE_ID" shell "printf '%s\n' '_ --remote-debugging-port=9222' > /data/local/tmp/chrome-command-line"
}

start_perfetto_trace() {
  local trace_remote="$1"
  local perfetto_output
  local perfetto_pid

  perfetto_output="$(
    adb -s "$DEVICE_ID" shell perfetto \
      --txt \
      --background \
      -c /data/misc/perfetto-configs/perfetto_config.pbtxt \
      -o "$trace_remote" 2>&1 || true
  )"

  perfetto_pid="$(printf '%s\n' "$perfetto_output" | sed -n 's/.*background pid: \([0-9][0-9]*\).*/\1/p' | head -n 1)"

  if [[ -z "$perfetto_pid" ]]; then
    perfetto_pid="$(adb -s "$DEVICE_ID" shell pidof perfetto 2>/dev/null | tr -d '\r' | awk '{print $1}' || true)"
  fi

  printf '%s\n' "$perfetto_output"
  return 0
}

# ── Chrome warmup (removes first-run bias) ───────────────────────
echo "→ Chrome warmup..."
adb -s "$DEVICE_ID" shell am force-stop com.android.chrome
sleep 1

# Write Chrome command-line flags (requires chrome://flags/#enable-command-line-on-non-rooted-devices).
# On non-rooted Android the first token should be "_" so Chrome treats the rest as flags.
write_chrome_command_line

adb -s "$DEVICE_ID" shell am start -a android.intent.action.VIEW -d "about:blank" >/dev/null
sleep 10
adb -s "$DEVICE_ID" shell am force-stop com.android.chrome

# ── Reset battery stats ──────────────────────────────────────────
adb -s "$DEVICE_ID" shell dumpsys battery reset >/dev/null 2>&1 || true

# ── Push Perfetto config ─────────────────────────────────────────
adb -s "$DEVICE_ID" shell mkdir -p /data/misc/perfetto-configs/
adb -s "$DEVICE_ID" push "$PERFETTO_CONFIG" /data/misc/perfetto-configs/ >/dev/null

# ── CDP forward (also re-set per run inside loop) ────────────────
adb -s "$DEVICE_ID" forward tcp:9222 localabstract:chrome_devtools_remote

# ── Randomised run order ─────────────────────────────────────────
python3 - <<EOF > "$RAW_DIR/run_order.txt"
import random

runs = $RUNS
scenarios = [s.strip() for s in "$SCENARIOS".split(",") if s.strip()]
if not scenarios:
    raise SystemExit("No scenarios configured.")

cases = []
for scenario in scenarios:
    cases.extend([("ssr", scenario)] * runs)
    cases.extend([("csr", scenario)] * runs)

random.shuffle(cases)
for condition, scenario in cases:
    print(f"{condition},{scenario}")
EOF

TOTAL_RUNS=$(wc -l < "$RAW_DIR/run_order.txt")
echo "Total runs scheduled: $TOTAL_RUNS"

echo "run,condition,scenario,temp_c,timestamp" > "$RAW_DIR/temperatures.csv"

RUN_NUM=0

while IFS=, read -r CONDITION SCENARIO <&3; do
  RUN_NUM=$((RUN_NUM + 1))

  URL=$([[ "$CONDITION" == "ssr" ]] && echo "$SSR_URL" || echo "$CSR_URL")
  PAGE_TEMPLATE=$([[ "$CONDITION" == "ssr" ]] && echo "$SSR_PAGES" || echo "$CSR_PAGES")
  PAGES="${PAGE_TEMPLATE//\{scenario\}/$SCENARIO}"

  TRACE_REMOTE="/data/misc/perfetto-traces/trace_${CONDITION}_${SCENARIO}_${RUN_NUM}"
  TRACE_LOCAL="$RAW_DIR/run_${RUN_NUM}_${CONDITION}_${SCENARIO}.perfetto-trace"
  METRICS_FILE="$RAW_DIR/run_${RUN_NUM}_${CONDITION}_${SCENARIO}.json"
  BATTERY_CSV="$RAW_DIR/run_${RUN_NUM}_${CONDITION}_${SCENARIO}.battery.csv"

  echo ""
  echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  echo "Run $RUN_NUM / $TOTAL_RUNS  ($CONDITION / $SCENARIO)"
  echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

  # ── 1. Reset to neutral state before cooldown ──────────────────
  adb -s "$DEVICE_ID" shell am force-stop com.android.chrome
  sleep 1
  adb -s "$DEVICE_ID" shell input keyevent KEYCODE_HOME
  sleep 1

  # ── 2. Cooldown ────────────────────────────────────────────────
  echo "→ Cooling down ${COOL_WAIT_S}s..."
  sleep "$COOL_WAIT_S"

  # ── 3. Temperature check ───────────────────────────────────────
  TEMP_RAW=$(adb -s "$DEVICE_ID" shell dumpsys battery | grep -m1 "temperature:" | awk '{print $2}' | tr -d '\r')
  [[ -z "$TEMP_RAW" ]] && TEMP_RAW=300
  TEMP_C=$(python3 -c "print(round($TEMP_RAW/10,1))")

  echo "$RUN_NUM,$CONDITION,$SCENARIO,$TEMP_C,$(date -u +%Y-%m-%dT%H:%M:%SZ)" >> "$RAW_DIR/temperatures.csv"
  echo "Temp: ${TEMP_C}°C"

  if (( $(echo "$TEMP_C > $MAX_TEMP_C" | bc -l) )); then
    echo "⚠ Too hot (>${MAX_TEMP_C}°C), skipping run..."
    continue
  fi

  # ── 4. Re-assert consistent launch state ───────────────────────
  adb -s "$DEVICE_ID" shell am force-stop com.android.chrome
  sleep 1
  adb -s "$DEVICE_ID" shell input keyevent KEYCODE_HOME
  sleep 1

  # Brightness consistency
  adb -s "$DEVICE_ID" shell settings put system screen_brightness 120

  # Re-write Chrome command-line flags each run (survives force-stop)
  write_chrome_command_line

  # Re-establish ADB tunnels each run (prevents silent drops mid-experiment)
  adb -s "$DEVICE_ID" reverse tcp:3000 tcp:3000 2>/dev/null || true
  adb -s "$DEVICE_ID" forward tcp:9222 localabstract:chrome_devtools_remote 2>/dev/null || true

  # ── 5. Start Perfetto ──────────────────────────────────────────
  PERFETTO_OUTPUT="$(start_perfetto_trace "$TRACE_REMOTE")"
  PERFETTO_PID="$(printf '%s\n' "$PERFETTO_OUTPUT" | sed -n 's/.*background pid: \([0-9][0-9]*\).*/\1/p' | head -n 1)"

  if [[ -z "$PERFETTO_PID" ]]; then
    PERFETTO_PID="$(adb -s "$DEVICE_ID" shell pidof perfetto 2>/dev/null | tr -d '\r' | awk '{print $1}' || true)"
  fi

  if [[ -z "$PERFETTO_PID" ]]; then
    echo "⚠ Could not get Perfetto PID — trace may be missing for this run"
    if [[ -n "$PERFETTO_OUTPUT" ]]; then
      echo "  perfetto start output:"
      printf '%s\n' "$PERFETTO_OUTPUT" | sed 's/^/    /'
    fi
  else
    echo "→ Perfetto started (pid: $PERFETTO_PID)"
  fi

  sleep 2

  # ── 6. Start battery poller ────────────────────────────────────
  start_battery_poller "$BATTERY_CSV"

  # ── 7. Run Puppeteer scenario ──────────────────────────────────
  node "$PUPPETEER_SCENARIO" \
    --url "$URL" \
    --condition "$CONDITION" \
    --scenario "$SCENARIO" \
    --run "$RUN_NUM" \
    --device-id "$DEVICE_ID" \
    --pages "$PAGES" \
    --out "$METRICS_FILE" \
    2>&1 | tee "$RAW_DIR/run_${RUN_NUM}_${CONDITION}_${SCENARIO}.log"

  stop_battery_poller

  BATTERY_QUALITY="$(summarize_battery_quality "$BATTERY_CSV")"
  BATTERY_SAMPLES="${BATTERY_QUALITY%%,*}"
  BATTERY_NONZERO_SAMPLES="${BATTERY_QUALITY##*,}"
  if [[ "${BATTERY_SAMPLES:-0}" -gt 0 && "${BATTERY_NONZERO_SAMPLES:-0}" -eq 0 ]]; then
    echo "⚠ Battery current samples were all zero for this run."
    echo "  Likely cause: the phone is still being powered over USB, so battery-based energy is not valid."
  fi

  # ── 8. Stop Perfetto ───────────────────────────────────────────
  if [[ -n "$PERFETTO_PID" ]]; then
    adb -s "$DEVICE_ID" shell kill -SIGINT "$PERFETTO_PID" 2>/dev/null || true
  else
    # Fallback: broad pkill (less safe but better than no stop)
    adb -s "$DEVICE_ID" shell pkill -SIGINT perfetto 2>/dev/null || true
  fi

  sleep 3

  # ── 9. Pull trace ──────────────────────────────────────────────
  if adb -s "$DEVICE_ID" pull "$TRACE_REMOTE" "$TRACE_LOCAL"; then
    adb -s "$DEVICE_ID" shell rm -f "$TRACE_REMOTE" 2>/dev/null || true
  else
    echo "⚠ First pull failed, retrying..."
    sleep 3
    if adb -s "$DEVICE_ID" pull "$TRACE_REMOTE" "$TRACE_LOCAL"; then
      adb -s "$DEVICE_ID" shell rm -f "$TRACE_REMOTE" 2>/dev/null || true
    else
      echo "✗ Could not pull trace for run $RUN_NUM — continuing"
    fi
  fi

  # ── 10. Sanity output ──────────────────────────────────────────
  if [[ -f "$METRICS_FILE" ]]; then
    LCP=$(python3 -c "
import json, sys
try:
    d = json.load(open('$METRICS_FILE'))
    pages = d.get('pages', [])
    val = (pages[0].get('metrics', {}) if pages else {}).get('lcp')
    print(f'{val:.0f}' if val else 'n/a')
except Exception as e:
    print('n/a')
" 2>/dev/null)
    echo "LCP: ${LCP} ms"
  fi

  if [[ -f "$TRACE_LOCAL" ]]; then
    SIZE=$(wc -c < "$TRACE_LOCAL" 2>/dev/null || echo 0)
    echo "Trace: $(echo "scale=1; $SIZE/1048576" | bc) MB"
  else
    echo "Trace: missing"
  fi

done 3< "$RAW_DIR/run_order.txt"

# ── Cleanup battery state ────────────────────────────────────────
adb -s "$DEVICE_ID" shell dumpsys battery reset >/dev/null 2>&1 || true

# ── Analysis ─────────────────────────────────────────────────────
echo ""
echo "→ Running analysis..."

python3 "$EXTRACT_ENERGY_SCRIPT" --raw-dir "$RAW_DIR"
python3 "$ANALYZE_RUNS_SCRIPT"   --raw-dir "$RAW_DIR"

# ── Move outputs ─────────────────────────────────────────────────
[[ -f "$RAW_DIR/energy_metrics.csv" ]] && mv -f "$RAW_DIR/energy_metrics.csv" "$PROCESSED_DIR/energy_metrics.csv"
[[ -f "$RAW_DIR/statistics.csv" ]]     && mv -f "$RAW_DIR/statistics.csv"     "$ANALYSIS_DIR/statistics.csv"
[[ -f "$RAW_DIR/report.md" ]]          && mv -f "$RAW_DIR/report.md"          "$ANALYSIS_DIR/report.md"
if [[ -d "$RAW_DIR/plots" ]]; then
  rm -rf "$ANALYSIS_DIR/plots"
  mv "$RAW_DIR/plots" "$ANALYSIS_DIR/plots"
fi

echo ""
echo "✓ Done"
echo "  raw:       $RAW_DIR"
echo "  processed: $PROCESSED_DIR"
echo "  analysis:  $ANALYSIS_DIR"

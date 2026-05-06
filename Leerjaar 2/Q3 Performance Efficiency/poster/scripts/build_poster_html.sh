#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
POSTER_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
HTML_FILE="${POSTER_DIR}/main.html"
PDF_FILE="${POSTER_DIR}/main.pdf"
CHROME_FALLBACK_PDF="${HOME}/Downloads/main.pdf"

find_chrome_bin() {
  if [[ -n "${CHROME_BIN:-}" ]]; then
    if [[ -x "${CHROME_BIN}" ]]; then
      printf '%s\n' "${CHROME_BIN}"
      return 0
    fi
    echo "CHROME_BIN is set but not executable: ${CHROME_BIN}" >&2
    exit 1
  fi

  local browser
  for browser in \
    google-chrome \
    google-chrome-stable \
    chromium \
    chromium-browser \
    chrome \
    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome" \
    "/Applications/Chromium.app/Contents/MacOS/Chromium"
  do
    if [[ "${browser}" = /* && -x "${browser}" ]]; then
      printf '%s\n' "${browser}"
      return 0
    fi
    if command -v "${browser}" >/dev/null 2>&1; then
      command -v "${browser}"
      return 0
    fi
  done

  echo "Could not find Chrome or Chromium. Set CHROME_BIN to the browser executable." >&2
  exit 1
}

make_preview_path() {
  if command -v mktemp >/dev/null 2>&1; then
    local preview_dir
    preview_dir="$(mktemp -d "${TMPDIR:-/tmp}/poster_preview.XXXXXX")"
    printf '%s/preview.png\n' "${preview_dir}"
    return 0
  fi

  printf '%s/poster_preview_%s.png\n' "${TMPDIR:-/tmp}" "$$"
}

convert_pdf_to_png() {
  local input_file="$1"
  local width="$2"
  local output_file="$3"

  if command -v sips >/dev/null 2>&1; then
    sips -s format png "${input_file}" --resampleWidth "${width}" --out "${output_file}" >/dev/null
    return 0
  fi

  if command -v magick >/dev/null 2>&1; then
    magick -density 300 "${input_file}[0]" -resize "${width}" "${output_file}" >/dev/null
    return 0
  fi

  if command -v pdftoppm >/dev/null 2>&1; then
    local prefix
    prefix="${output_file%.png}"
    pdftoppm -png -singlefile -scale-to "${width}" "${input_file}" "${prefix}" >/dev/null
    return 0
  fi

  echo "Skipping ${input_file}: install sips, ImageMagick, or pdftoppm for PNG export." >&2
}

CHROME_BIN="$(find_chrome_bin)"
PREVIEW_FILE="${PREVIEW_FILE:-$(make_preview_path)}"

cd "${POSTER_DIR}"

if [[ -f figures/energy-medians.pdf ]]; then
  convert_pdf_to_png figures/energy-medians.pdf 2200 figures/energy-medians.png
fi

if [[ -f figures/browser-tradeoffs.pdf ]]; then
  convert_pdf_to_png figures/browser-tradeoffs.pdf 1800 figures/browser-tradeoffs.png
fi

if [[ -f figures/energy-boxplots.pdf ]]; then
  convert_pdf_to_png figures/energy-boxplots.pdf 1800 figures/energy-boxplots.png
fi

"${CHROME_BIN}" \
  --headless=new \
  --disable-gpu \
  --allow-file-access-from-files \
  --run-all-compositor-stages-before-draw \
  --virtual-time-budget=2000 \
  --window-size=1920,1080 \
  --screenshot="${PREVIEW_FILE}" \
  "file://${HTML_FILE}" >/dev/null 2>&1

"${CHROME_BIN}" \
  --headless=new \
  --disable-gpu \
  --allow-file-access-from-files \
  --run-all-compositor-stages-before-draw \
  --virtual-time-budget=2000 \
  --window-size=1920,1080 \
  --print-to-pdf-no-header \
  --print-to-pdf="${PDF_FILE}" \
  "file://${HTML_FILE}" >/dev/null 2>&1

if [[ ! -f "${PDF_FILE}" && -f "${CHROME_FALLBACK_PDF}" ]]; then
  mv "${CHROME_FALLBACK_PDF}" "${PDF_FILE}"
fi

if [[ ! -f "${PDF_FILE}" ]]; then
  echo "Chrome did not produce ${PDF_FILE}" >&2
  exit 1
fi

echo "Built ${PDF_FILE}"
echo "Preview ${PREVIEW_FILE}"

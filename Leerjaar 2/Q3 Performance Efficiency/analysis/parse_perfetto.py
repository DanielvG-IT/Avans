#!/usr/bin/env python3
"""
Backward-compatible wrapper. Prefer:
  python3 analysis/extract_energy.py --results-dir <raw-run-dir>
"""

from extract_energy import main


if __name__ == "__main__":
    main()

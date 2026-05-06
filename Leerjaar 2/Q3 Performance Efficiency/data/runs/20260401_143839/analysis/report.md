# SSR vs CSR Benchmark — Results

**Device:** Samsung SM-A536B (Galaxy A53), Android 16, SDK 36  
**Direct energy method:** Battery current × voltage integration (250ms polling)  
**Proxy energy method:** Perfetto CPU frequency residency weighted by core class  
**Statistical test:** Mann-Whitney U, two-sided, α = 0.05  
**Bonferroni-corrected threshold:** α = 0.0019 (27 tests)  

## Results table

| Scenario | Metric | SSR median (IQR) | CSR median (IQR) | *p* | Effect *r* | Sig |
|----------|--------|------------------|------------------|-----|-----------|-----|
| dynamic | Total energy | 6372.2 (±1684.6) | 7040.3 (±1281.9) | 0.2730 | 0.300 (medium) | – |
| dynamic | Average power | 836.5 (±206.8) | 773.6 (±91.6) | 0.2413 | -0.320 (medium) | – |
| dynamic | LCP — cold load | 3302.0 (±16.0) | 3270.0 (±25.0) | 0.0010 | -0.870 (large) | ✓ |
| dynamic | FCP — cold load | 3302.0 (±16.0) | 3270.0 (±25.0) | 0.0010 | -0.870 (large) | ✓ |
| dynamic | TTFB — cold load | 31.2 (±76.8) | 22.1 (±46.0) | 0.1859 | -0.360 (medium) | – |
| dynamic | Total Blocking Time (all pages) | 0.0 (±0.0) | 0.0 (±0.0) | 1.0000 | 0.000 (negligible) | – |
| dynamic | CLS — cold load | 0.0 (±0.0) | 0.0 (±0.0) | 1.0000 | 0.000 (negligible) | – |
| dynamic | Transfer size (all pages) | 7.1 (±0.0) | 3.9 (±0.0) | <0.0001 | -1.000 (large) | ✓ |
| dynamic | JS heap peak | 9.5 (±0.0) | 9.5 (±0.0) | 1.0000 | 0.000 (negligible) | – |
| massive | Total energy | 5769.5 (±401.8) | 9047.8 (±18662.8) | 0.0008 | 0.900 (large) | ✓ |
| massive | Average power | 768.7 (±73.8) | 1044.1 (±2242.0) | 0.0211 | 0.620 (large) | – |
| massive | LCP — cold load | 3296.0 (±16.0) | 3274.0 (±26.0) | 0.0532 | -0.520 (large) | – |
| massive | FCP — cold load | 3296.0 (±16.0) | 3274.0 (±26.0) | 0.0532 | -0.520 (large) | – |
| massive | TTFB — cold load | 67.8 (±83.8) | 21.9 (±139.4) | 0.1405 | -0.400 (medium) | – |
| massive | Total Blocking Time (all pages) | 0.0 (±0.0) | 0.0 (±0.0) | 1.0000 | 0.000 (negligible) | – |
| massive | CLS — cold load | 0.0 (±0.0) | 0.0 (±0.0) | 1.0000 | 0.000 (negligible) | – |
| massive | Transfer size (all pages) | 7.1 (±0.0) | 3.9 (±0.0) | <0.0001 | -1.000 (large) | ✓ |
| massive | JS heap peak | 9.5 (±0.0) | 13.6 (±0.0) | <0.0001 | 1.000 (large) | ✓ |
| static | Total energy | 5742.9 (±378.8) | 7960.1 (±2159.5) | 0.0013 | 0.860 (large) | ✓ |
| static | Average power | 764.0 (±30.2) | 855.3 (±232.9) | 0.0028 | 0.800 (large) | – |
| static | LCP — cold load | 3292.0 (±16.0) | 3290.0 (±15.0) | 1.0000 | 0.000 (negligible) | – |
| static | FCP — cold load | 3286.0 (±18.0) | 3290.0 (±15.0) | 0.6203 | 0.140 (small) | – |
| static | TTFB — cold load | 24.1 (±17.6) | 80.7 (±135.0) | 0.9698 | -0.020 (negligible) | – |
| static | Total Blocking Time (all pages) | 0.0 (±0.0) | 0.0 (±0.0) | 1.0000 | 0.000 (negligible) | – |
| static | CLS — cold load | 0.0 (±0.0) | 0.0 (±0.0) | 1.0000 | 0.000 (negligible) | – |
| static | Transfer size (all pages) | 7.2 (±0.0) | 3.9 (±0.0) | <0.0001 | -1.000 (large) | ✓ |
| static | JS heap peak | 9.5 (±0.0) | 9.5 (±0.0) | 1.0000 | 0.000 (negligible) | – |

## Methodology notes

- All runs interleaved in randomised order to prevent thermal session bias.
- Chrome force-stopped before each run; no app-data wipe between runs.
- Screen brightness fixed at 120/255 (manual mode) throughout.
- Device radios: WiFi only (mobile data, Bluetooth, NFC disabled).
- Device idle ≥45s between runs; runs discarded if start temp >37°C.
- Energy computed as ∫ |I(t)| × V(t) dt over trace window (trapezoidal rule).
- Battery-current energy under USB-connected runs may be contaminated by external power and should be treated cautiously when many samples are zero-current.
- CPU energy proxy is an inference from Perfetto CPU frequency residency, weighted more heavily for big cores; it supports relative SSR/CSR comparison, not absolute joule claims.
- SM-A536B does not expose hardware power rail counters; battery polling used.
- Runs without a working CDP connection remain usable for system-level trace and energy analysis, but browser-level metrics are excluded when unavailable.

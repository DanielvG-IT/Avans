#!/usr/bin/env node
// =============================================================================
// Browser scenario via Chrome DevTools Protocol
//
// Works by forwarding the Chrome CDP port over ADB:
//   adb forward tcp:9222 localabstract:chrome_devtools_remote
// (run.sh handles this automatically)
//
// IMPORTANT — Samsung A53 note:
// Chrome on Samsung One UI may show a "Chrome isn't your default browser"
// banner on first ever launch after factory reset you may need to manually
// dismiss it once. This steals focus and can delay metrics!
//
// Usage (called by run.sh — not run manually):
//   node scripts/puppeteer-scenario.js \
//     --url http://192.168.x.x:3000 \
//     --condition ssr \
//     --run 1 \
//     --out data/runs/YYYYMMDD_HHMMSS/raw/run_1_ssr.json
// =============================================================================

import { connect } from "puppeteer-core";
import { writeFileSync } from "fs";
import { execSync } from "child_process";

// ── Parse args ────────────────────────────────────────────────────────────────
const argv = {};
for (let i = 2; i < process.argv.length - 1; i += 2) {
  const rawKey = process.argv[i].replace("--", "");
  argv[rawKey] = process.argv[i + 1];

  // Accept both kebab-case and camelCase (e.g. --device-id and --deviceId).
  const camelKey = rawKey.replace(/-([a-z])/g, (_, c) => c.toUpperCase());
  argv[camelKey] = process.argv[i + 1];
}
const { url, condition, scenario, run, out, deviceId } = argv;
if (!url || !condition || !run || !out) {
  process.stderr.write(
    "Missing required args: --url --condition --run --out\n",
  );
  process.exit(1);
}

const ADB_PREFIX = deviceId ? `adb -s "${deviceId}"` : "adb";

// ── Pages to navigate ─────────────────────────────────────────────────────────
// Thesis-quality default: isolate each condition to its own route set.
const defaultPagesByCondition = {
  ssr: "/ssr",
  csr: "/csr",
};

const pagesArg = argv.pages || defaultPagesByCondition[condition] || "/";

const PAGES = pagesArg
  .split(",")
  .map((p) => p.trim())
  .filter(Boolean);

const DWELL_MS = 1000; // Time to "read" each page before navigating
const LOAD_TIMEOUT_MS = 30000;
const SETTLE_MS_CSR = 800; // Extra settle for React hydration on CSR
const CDP_READY_TIMEOUT_MS = 15000;
const CDP_POLL_MS = 500;
const FALLBACK_PAGE_OBSERVE_MS = 5000;
const CHROME_BOOT_URL = "about:blank";

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function runCommand(command, options = {}) {
  try {
    const stdout = execSync(command, {
      stdio: "pipe",
      encoding: "utf8",
      ...options,
    });
    return { ok: true, stdout: stdout.trim() };
  } catch (error) {
    return {
      ok: false,
      stdout: error.stdout?.toString().trim() || "",
      stderr: error.stderr?.toString().trim() || error.message,
    };
  }
}

// ── Helpers ───────────────────────────────────────────────────────────────────
async function getMetrics(page) {
  // Performance timing, paint, LCP, TBT, CLS, resource summary
  const data = await page.evaluate(() => {
    const nav = performance.getEntriesByType("navigation")[0];
    const paint = performance.getEntriesByType("paint");
    const lcp = performance.getEntriesByType("largest-contentful-paint");
    const lt = performance.getEntriesByType("longtask");
    const ls = performance.getEntriesByType("layout-shift");
    const resources = performance.getEntriesByType("resource");

    return {
      ttfb: nav ? nav.responseStart - nav.requestStart : null,
      domContentLoaded: nav
        ? nav.domContentLoadedEventEnd - nav.startTime
        : null,
      loadComplete: nav ? nav.loadEventEnd - nav.startTime : null,
      transferSize: nav ? nav.transferSize : null,

      firstPaint:
        paint.find((e) => e.name === "first-paint")?.startTime ?? null,
      firstContentfulPaint:
        paint.find((e) => e.name === "first-contentful-paint")?.startTime ??
        null,
      lcp: lcp.length > 0 ? lcp[lcp.length - 1].startTime : null,

      // Total Blocking Time = sum of (task_duration - 50ms) for long tasks
      tbt: lt.reduce((s, t) => s + Math.max(0, t.duration - 50), 0),

      // Cumulative Layout Shift
      cls: ls.reduce((s, e) => s + (e.value || 0), 0),

      resources: resources.map((r) => ({
        name: r.name.split("/").slice(-2).join("/"),
        type: r.initiatorType,
        size_kb: Math.round(r.transferSize / 1024),
        duration: Math.round(r.duration),
      })),

      jsHeap: performance.memory
        ? {
            used_mb:
              Math.round((performance.memory.usedJSHeapSize / 1048576) * 10) /
              10,
            total_mb:
              Math.round((performance.memory.totalJSHeapSize / 1048576) * 10) /
              10,
          }
        : null,
    };
  });

  return data;
}

async function waitForCdpReady(timeoutMs, diagnostics) {
  const start = Date.now();
  const endpoint = "http://localhost:9222/json/version";

  while (Date.now() - start < timeoutMs) {
    try {
      const res = await fetch(endpoint);
      if (res.ok) {
        diagnostics.versionProbe = {
          ok: true,
          status: res.status,
          body: await res.text(),
        };
        return true;
      }
      diagnostics.versionProbe = {
        ok: false,
        status: res.status,
        body: await res.text(),
      };
    } catch (_) {
      diagnostics.versionProbe = {
        ok: false,
        error: _.message,
      };
    }
    await sleep(CDP_POLL_MS);
  }

  return false;
}

async function runFallbackScenario(result) {
  process.stdout.write(
    "  [puppeteer] Falling back to deterministic am-start protocol\n",
  );

  result.meta.protocol_mode = "fallback_am_start";
  result.meta.browser_metrics_available = false;
  result.meta.fallback = {
    page_observe_ms: FALLBACK_PAGE_OBSERVE_MS,
    dwell_ms: DWELL_MS,
  };

  for (let i = 0; i < PAGES.length; i += 1) {
    const pageUrl = `${url.replace(/\/$/, "")}${PAGES[i]}`;
    const launch = runCommand(
      `${ADB_PREFIX} shell am start -W -a android.intent.action.VIEW -d "${pageUrl}"`,
      {
        timeout: 15000,
      },
    );

    result.pages.push({
      page_index: i,
      path: PAGES[i],
      nav_ms: null,
      metrics: null,
      fallback_launch: launch.ok ? "ok" : "error",
      fallback_stdout: launch.stdout || null,
      fallback_stderr: launch.stderr || null,
    });

    if (!launch.ok) {
      result.errors.push({
        message: `am start fallback failed for ${pageUrl}: ${launch.stderr || "unknown error"}`,
      });
      continue;
    }

    await sleep(FALLBACK_PAGE_OBSERVE_MS);
    await sleep(DWELL_MS);
  }
}

// ── Main ──────────────────────────────────────────────────────────────────────
(async () => {
  const result = {
    meta: {
      run: parseInt(run),
      condition,
      scenario: scenario || "default",
      url,
      timestamp: new Date().toISOString(),
      pages_tested: PAGES,
      protocol_mode: "cdp",
      browser_metrics_available: true,
    },
    pages: [],
    errors: [],
    diagnostics: {
      adbForwardList: null,
      chromeSocket: null,
      chromeLaunch: null,
      versionProbe: null,
      puppeteerConnect: null,
    },
  };

  let browser = null;

  try {
    // Ensure Chrome is launched before connecting to CDP.
    // Launch a neutral page first so the measured route is the first experimental navigation.
    // On Android, the devtools socket may not exist until Chrome is running.
    result.diagnostics.adbForwardList = runCommand(`${ADB_PREFIX} forward --list`);
    result.diagnostics.chromeSocket = runCommand(
      `${ADB_PREFIX} shell cat /proc/net/unix | grep chrome_devtools_remote`,
    );

    result.diagnostics.chromeLaunch = runCommand(
      `${ADB_PREFIX} shell am start -a android.intent.action.VIEW -d "${CHROME_BOOT_URL}"`,
      {
        timeout: 5000,
      },
    );
    if (!result.diagnostics.chromeLaunch.ok) {
      throw new Error(
        `Chrome launch failed: ${result.diagnostics.chromeLaunch.stderr || "unknown error"}`,
      );
    }

    const cdpReady = await waitForCdpReady(
      CDP_READY_TIMEOUT_MS,
      result.diagnostics,
    );
    if (!cdpReady) {
      throw new Error(
        "DevTools endpoint not ready at http://localhost:9222/json/version",
      );
    }

    // Connect to Chrome on device via DevTools Protocol.
    browser = await connect({
      browserURL: "http://localhost:9222",
      defaultViewport: { width: 390, height: 844 }, // Mobile viewport
    });
    result.diagnostics.puppeteerConnect = { ok: true };

    // Close pre-existing tabs so each run starts from one controlled page.
    const existingPages = await browser.pages();
    await Promise.all(
      existingPages.map((p) =>
        p.close({ runBeforeUnload: false }).catch(() => {}),
      ),
    );

    const page = await browser.newPage();

    // Intercept Chrome's "set as default browser" popup by dismissing dialogs
    page.on("dialog", async (dialog) => {
      await dialog.dismiss().catch(() => {});
    });

    // Observe LCP and CLS from the start
    await page.evaluateOnNewDocument(() => {
      window.__obs = { lcp: null, cls: 0 };
      new PerformanceObserver((list) => {
        const e = list.getEntries();
        if (e.length) window.__obs.lcp = e[e.length - 1].startTime;
      }).observe({ type: "largest-contentful-paint", buffered: true });
      new PerformanceObserver((list) => {
        list.getEntries().forEach((e) => {
          window.__obs.cls += e.value || 0;
        });
      }).observe({ type: "layout-shift", buffered: true });
    });

    for (let i = 0; i < PAGES.length; i++) {
      const pageUrl = `${url.replace(/\/$/, "")}${PAGES[i]}`;
      process.stdout.write(
        `  [puppeteer] ${i + 1}/${PAGES.length} → ${pageUrl}\n`,
      );

      const t0 = Date.now();

      await page.goto(pageUrl, {
        waitUntil: "networkidle2",
        timeout: LOAD_TIMEOUT_MS,
      });

      // CSR apps need extra time for React hydration after networkidle2
      if (condition === "csr") {
        await sleep(SETTLE_MS_CSR);
      }

      const metrics = await getMetrics(page);
      const obs = await page.evaluate(() => window.__obs || {});

      // LCP from PerformanceObserver is more reliable than from getEntries()
      // because it fires asynchronously after paint
      if (obs.lcp && !metrics.lcp) {
        metrics.lcp = obs.lcp;
      }

      result.pages.push({
        page_index: i,
        path: PAGES[i],
        nav_ms: Date.now() - t0,
        metrics,
      });

      process.stdout.write(
        `  [puppeteer]   LCP=${metrics.lcp?.toFixed(0) ?? "n/a"}ms  ` +
          `FCP=${metrics.firstContentfulPaint?.toFixed(0) ?? "n/a"}ms  ` +
          `TBT=${metrics.tbt?.toFixed(0) ?? "n/a"}ms  ` +
          `TTFB=${metrics.ttfb?.toFixed(0) ?? "n/a"}ms\n`,
      );

      // Dwell time (simulate user reading)
      await sleep(DWELL_MS);
    }

    await page.close();
  } catch (err) {
    process.stderr.write(`  [puppeteer] ERROR: ${err.message}\n`);
    result.errors.push({ message: err.message });
    if (!result.diagnostics.puppeteerConnect) {
      result.diagnostics.puppeteerConnect = {
        ok: false,
        error: err.message,
      };
    }

    if (url) {
      await runFallbackScenario(result);
    }
  } finally {
    if (browser) {
      await browser.disconnect().catch(() => {});
    }
  }

  writeFileSync(out, JSON.stringify(result, null, 2));
  process.stdout.write(`  [puppeteer] ✓ Saved → ${out}\n`);
})();

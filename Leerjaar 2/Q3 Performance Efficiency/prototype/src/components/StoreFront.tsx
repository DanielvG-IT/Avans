"use client";

import {
  BenchmarkQuery,
  categories,
  DecoratedProduct,
  StoreSummary,
} from "@/data/mockData";

export const StoreFront = ({
  products,
  query,
  totalItems,
  page,
  totalPages,
  summary,
  onQueryChange,
  onPageChange,
  basePath,
  isClient,
  isLoading = false,
}: {
  products: DecoratedProduct[];
  query: BenchmarkQuery;
  totalItems: number;
  page: number;
  totalPages: number;
  summary: StoreSummary;
  onQueryChange?: (updates: Partial<BenchmarkQuery>) => void;
  onPageChange?: (nextPage: number) => void;
  basePath?: string;
  isClient: boolean;
  isLoading?: boolean;
}) => {
  const navigateWithQuery = (updates: Partial<BenchmarkQuery>) => {
    if (!basePath) {
      return;
    }

    const next = { ...query, ...updates };
    const params = new URLSearchParams();
    params.set("scenario", next.scenario);
    params.set("q", next.q);
    params.set("category", next.category);
    params.set("sort", next.sort);
    params.set("page", String(next.page));
    window.location.assign(`${basePath}?${params.toString()}`);
  };

  const emitQueryChange = (updates: Partial<BenchmarkQuery>) => {
    if (onQueryChange) {
      onQueryChange(updates);
      return;
    }

    navigateWithQuery(updates);
  };

  const emitPageChange = (nextPage: number) => {
    if (onPageChange) {
      onPageChange(nextPage);
      return;
    }

    navigateWithQuery({ page: nextPage });
  };

  return (
    <div className="bg-white min-h-screen text-slate-900 font-sans">
      <header className="sticky top-0 z-50 bg-white/80 backdrop-blur-md border-b border-gray-100 p-6">
        <div className="max-w-7xl mx-auto flex flex-col md:flex-row justify-between items-center gap-4">
          <h1 className="text-xl font-semibold tracking-widest uppercase">
            NORDIC
          </h1>
          <div className="grid w-full gap-3 md:grid-cols-5">
            <select
              className="w-full bg-gray-50 border-none rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-black"
              value={query.scenario}
              onChange={(e) =>
                emitQueryChange({
                  scenario: e.target.value as BenchmarkQuery["scenario"],
                  page: 1,
                })
              }>
              <option value="static">Static case</option>
              <option value="dynamic">Dynamic case</option>
              <option value="massive">Dynamic + large dataset</option>
            </select>
            <input
              type="text"
              placeholder="Search inventory..."
              className="w-full bg-gray-50 border-none rounded-lg px-4 py-2 text-sm focus:ring-2 focus:ring-black transition-all"
              value={query.q}
              onChange={(e) => emitQueryChange({ q: e.target.value, page: 1 })}
            />
            <select
              className="w-full bg-gray-50 border-none rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-black"
              value={query.category}
              onChange={(e) =>
                emitQueryChange({ category: e.target.value, page: 1 })
              }>
              {categories.map((category) => (
                <option key={category} value={category}>
                  {category}
                </option>
              ))}
            </select>
            <select
              className="w-full bg-gray-50 border-none rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-black"
              value={query.sort}
              onChange={(e) =>
                emitQueryChange({
                  sort: e.target.value as BenchmarkQuery["sort"],
                  page: 1,
                })
              }>
              <option value="name-asc">Name A-Z</option>
              <option value="price-asc">Price low-high</option>
              <option value="price-desc">Price high-low</option>
            </select>
            <span className="text-[10px] text-gray-400 self-center md:justify-self-end">
              {isClient ? "⚡ CLIENT-SIDE" : "🌐 SERVER-SIDE"}
            </span>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto p-8">
        <div className="flex justify-between items-baseline mb-8">
          <h2 className="text-3xl font-light">New Arrivals</h2>
          <span className="text-gray-400 text-sm">
            {totalItems} filtered items, page {page} / {totalPages}
          </span>
        </div>

        <section className="mb-10 grid gap-4 md:grid-cols-4">
          <div className="rounded-2xl border border-slate-100 bg-slate-50 p-5">
            <p className="text-[11px] uppercase tracking-[0.24em] text-slate-400">
              Scenario
            </p>
            <p className="mt-2 text-lg font-semibold text-slate-900">
              {summary.scenarioLabel}
            </p>
            <p className="mt-2 text-sm text-slate-500">
              {summary.datasetSize.toLocaleString()} source records
            </p>
          </div>
          <div className="rounded-2xl border border-slate-100 bg-slate-50 p-5">
            <p className="text-[11px] uppercase tracking-[0.24em] text-slate-400">
              Average ticket
            </p>
            <p className="mt-2 text-lg font-semibold text-slate-900">
              €{summary.averagePrice.toFixed(2)}
            </p>
            <p className="mt-2 text-sm text-slate-500">
              {summary.premiumCount.toLocaleString()} premium items
            </p>
          </div>
          <div className="rounded-2xl border border-slate-100 bg-slate-50 p-5">
            <p className="text-[11px] uppercase tracking-[0.24em] text-slate-400">
              Fulfilment
            </p>
            <p className="mt-2 text-lg font-semibold text-slate-900">
              {summary.expressReadyCount.toLocaleString()} express-ready
            </p>
            <p className="mt-2 text-sm text-slate-500">
              Client and server render the same final catalog state
            </p>
          </div>
          <div className="rounded-2xl border border-slate-100 bg-slate-50 p-5">
            <p className="text-[11px] uppercase tracking-[0.24em] text-slate-400">
              Merch index
            </p>
            <p className="mt-2 text-lg font-semibold text-slate-900">
              {summary.merchIndex.toFixed(1)}
            </p>
            <p className="mt-2 text-sm text-slate-500">
              Derived from catalog-wide ranking signals
            </p>
          </div>
        </section>

        <section className="mb-10 flex flex-wrap gap-3">
          {summary.categoryBreakdown.map((entry) => (
            <div
              key={entry.name}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm"
              style={{ backgroundColor: entry.accentColor }}>
              <span className="font-medium text-slate-800">{entry.name}</span>
              <span className="ml-2 text-slate-600">
                {entry.count} items • {entry.share.toFixed(1)}%
              </span>
            </div>
          ))}
        </section>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 xl:grid-cols-5 gap-8">
          {isLoading && products.length === 0
            ? Array.from({ length: 10 }, (_, index) => (
                <div
                  key={`loading-${index}`}
                  className="animate-pulse rounded-2xl border border-slate-100 p-4">
                  <div className="mb-4 aspect-4/5 rounded-2xl bg-slate-100" />
                  <div className="h-4 rounded bg-slate-100" />
                  <div className="mt-3 h-3 rounded bg-slate-100" />
                  <div className="mt-6 h-8 rounded bg-slate-100" />
                </div>
              ))
            : products.map((p) => (
                <article
                  key={p.id}
                  className="group cursor-pointer rounded-2xl border border-slate-100 p-4 shadow-sm shadow-slate-100/60 transition-transform duration-300 hover:-translate-y-1">
                  <div
                    className="aspect-4/5 rounded-2xl mb-4 transition-transform duration-500 group-hover:scale-[1.02]"
                    style={{ backgroundColor: p.accentColor }}
                  />
                  <div className="mb-3 flex items-center justify-between gap-3">
                    <span className="rounded-full bg-slate-100 px-2 py-1 text-[11px] font-medium uppercase tracking-[0.18em] text-slate-500">
                      {p.category}
                    </span>
                    <span className="text-xs text-slate-400">
                      {p.reviewCount} reviews
                    </span>
                  </div>
                  <h3 className="font-medium text-sm">{p.name}</h3>
                  <p className="mt-1 text-gray-400 text-xs">
                    {p.description}
                  </p>
                  <div className="mt-4 space-y-2 text-xs text-slate-500">
                    <p>{p.inventoryLabel}</p>
                    <p>{p.shippingLabel}</p>
                    <p>Merch score {p.merchScore.toFixed(1)}</p>
                  </div>
                  <div className="mt-5 flex items-end justify-between gap-4">
                    <div>
                      <p className="text-sm font-semibold">€{p.price}</p>
                      <p className="text-xs text-slate-400">{p.priceWithTax} incl. VAT</p>
                    </div>
                    <button
                      type="button"
                      className="rounded-full bg-slate-900 px-3 py-2 text-xs font-medium text-white">
                      Add
                    </button>
                  </div>
                </article>
              ))}
        </div>

        <div className="mt-10 flex items-center justify-center gap-3">
          <button
            type="button"
            className="rounded-lg border border-slate-200 px-4 py-2 text-sm disabled:opacity-40"
            disabled={page <= 1}
            onClick={() => emitPageChange(page - 1)}>
            Previous
          </button>
          <button
            type="button"
            className="rounded-lg border border-slate-200 px-4 py-2 text-sm disabled:opacity-40"
            disabled={page >= totalPages}
            onClick={() => emitPageChange(page + 1)}>
            Next
          </button>
        </div>
      </main>
    </div>
  );
};

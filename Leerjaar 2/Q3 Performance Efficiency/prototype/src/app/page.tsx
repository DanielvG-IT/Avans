export default function Home() {
  const scenarios = [
    { key: "static", label: "Static" },
    { key: "dynamic", label: "Dynamic" },
    { key: "massive", label: "Dynamic + Large Data" },
  ];

  return (
    <main className="min-h-screen bg-linear-to-br from-slate-50 to-slate-200 flex items-center justify-center p-6">
      <div className="w-full max-w-4xl rounded-2xl bg-white shadow-xl p-8 text-center">
        <h1 className="text-3xl font-bold text-slate-800 mb-2">
          Rendering Demo
        </h1>
        <p className="text-slate-500 mb-8">
          Compare equal CSR and SSR flows across multiple scenarios.
        </p>

        <div className="grid gap-4 sm:grid-cols-3">
          {scenarios.map((scenario) => (
            <div
              key={scenario.key}
              className="rounded-xl border border-slate-100 p-4 text-left">
              <h2 className="text-sm font-semibold text-slate-700 mb-3">
                {scenario.label}
              </h2>
              <div className="flex gap-2">
                <a
                  href={`/ssr?scenario=${scenario.key}`}
                  className="inline-flex flex-1 items-center justify-center rounded-lg bg-blue-600 px-4 py-2 text-white text-sm font-semibold hover:bg-blue-700 transition-colors">
                  SSR
                </a>
                <a
                  href={`/csr?scenario=${scenario.key}`}
                  className="inline-flex flex-1 items-center justify-center rounded-lg bg-emerald-600 px-4 py-2 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors">
                  CSR
                </a>
              </div>
            </div>
          ))}
        </div>
      </div>
    </main>
  );
}

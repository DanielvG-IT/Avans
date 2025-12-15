"use client";

import { Link, useNavigate } from "react-router-dom";

export default function NotFound(): React.ReactNode {
  const navigate = useNavigate();

  return (
    <main
      role="main"
      aria-labelledby="notfound-title"
      className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary/20 via-primary/10 to-accent/20 dark:from-primary/10 dark:via-background dark:to-accent/10 p-6"
    >
      <div className="w-full max-w-4xl bg-card/90 border border-border rounded-3xl p-8 sm:p-12 backdrop-blur-sm flex flex-col md:flex-row items-center gap-8 shadow-lg">
        <div className="flex-1 flex items-center justify-center">
          {/* Simple, lightweight illustrative SVG */}
          <svg
            width="220"
            height="160"
            viewBox="0 0 220 160"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            aria-hidden="true"
            className="opacity-95"
          >
            <defs>
              <linearGradient id="g1" x1="0" x2="1">
                <stop offset="0" stopColor="currentColor" stopOpacity="0.2" />
                <stop offset="1" stopColor="currentColor" stopOpacity="0.1" />
              </linearGradient>
            </defs>

            <rect
              x="8"
              y="12"
              width="204"
              height="116"
              rx="20"
              fill="url(#g1)"
              className="text-muted-foreground"
            />
            <g
              transform="translate(30,18)"
              fill="none"
              stroke="currentColor"
              strokeWidth="6"
              strokeLinecap="round"
              strokeLinejoin="round"
              opacity="0.95"
              className="text-muted-foreground"
            >
              <path d="M20 80C20 80 36 28 92 28C148 28 164 80 164 80" />
              <text
                x="56"
                y="76"
                fill="currentColor"
                fontWeight="800"
                fontSize="48"
                className="fill-foreground"
              >
                404
              </text>
            </g>
          </svg>
        </div>

        <section className="flex-1 text-center md:text-left" aria-live="polite">
          <h1
            id="notfound-title"
            className="text-5xl sm:text-6xl md:text-7xl font-extrabold tracking-tight leading-none"
          >
            <span className="text-foreground">404</span>
          </h1>

          <h2 className="mt-3 text-2xl sm:text-3xl font-semibold text-foreground">
            Page not found
          </h2>

          <p className="mt-2 text-muted-foreground max-w-xl">
            The page you're looking for doesn't exist or has moved. You can go back to the previous
            page, return home, or contact support if you need help.
          </p>

          <div className="mt-6 flex flex-col sm:flex-row sm:items-center gap-3 justify-center md:justify-between">
            <button
              type="button"
              onClick={() => navigate(-1)}
              className="inline-flex items-center justify-center px-5 py-3 rounded-lg bg-secondary hover:bg-secondary/80 border border-border text-secondary-foreground font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2"
            >
              Go back
            </button>

            <Link
              to="/"
              className="inline-flex items-center justify-center px-5 py-3 rounded-lg bg-primary hover:bg-primary/90 text-primary-foreground font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2"
            >
              Home
            </Link>
          </div>
        </section>
      </div>
    </main>
  );
}

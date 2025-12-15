import svgr from "vite-plugin-svgr";
import { VitePWA } from "vite-plugin-pwa";
import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react-swc";
import { imagetools } from "vite-imagetools";
import { defineConfig, loadEnv } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";
import viteCompression from "vite-plugin-compression";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  const isProd = mode === "production";

  return {
    root: process.cwd(),
    base: env.VITE_BASE ?? "/",

    plugins: [
      tailwindcss(),
      svgr(),
      react(),
      imagetools(),
      tsconfigPaths(),

      // PWA (registers service worker & manifest)
      VitePWA({
        registerType: "autoUpdate",
        includeAssets: ["favicon.ico", "robots.txt", "apple-touch-icon.png"],
        workbox: {
          globPatterns: ["**/*.{js,css,html,ico,png,svg,webp}"],
          runtimeCaching: [
            {
              urlPattern: /^https:\/\/fonts\.googleapis\.com\/.*/i,
              handler: "CacheFirst",
              options: {
                cacheName: "google-fonts-cache",
                expiration: {
                  maxEntries: 10,
                  maxAgeSeconds: 60 * 60 * 24 * 365, // 1 year
                },
                cacheableResponse: {
                  statuses: [0, 200],
                },
              },
            },
            {
              urlPattern: /^https:\/\/fonts\.gstatic\.com\/.*/i,
              handler: "CacheFirst",
              options: {
                cacheName: "gstatic-fonts-cache",
                expiration: {
                  maxEntries: 10,
                  maxAgeSeconds: 60 * 60 * 24 * 365, // 1 year
                },
                cacheableResponse: {
                  statuses: [0, 200],
                },
              },
            },
            {
              urlPattern: /\.(?:png|jpg|jpeg|svg|gif|webp)$/,
              handler: "CacheFirst",
              options: {
                cacheName: "images-cache",
                expiration: {
                  maxEntries: 50,
                  maxAgeSeconds: 60 * 60 * 24 * 30, // 30 days
                },
              },
            },
          ],
        },
        manifest: {
          name: env.VITE_APP_NAME ?? "Avans Keuzekompas",
          short_name: env.VITE_APP_SHORT_NAME ?? "Keuzekompas",
          description: "Avans Keuzekompas - Explore and manage your elective courses",
          start_url: "/",
          scope: "/",
          display: "standalone",
          orientation: "any",
          background_color: "#ffffff",
          theme_color: "#c6002a",
          categories: ["education", "productivity"],
          icons: [
            {
              src: "pwa-192x192.png",
              sizes: "192x192",
              type: "image/png",
              purpose: "any maskable",
            },
            {
              src: "pwa-512x512.png",
              sizes: "512x512",
              type: "image/png",
              purpose: "any maskable",
            },
          ],
        },
      }),

      // Compression: gzip + brotli for production builds
      viteCompression({
        algorithm: "gzip",
        ext: ".gz",
        deleteOriginFile: false,
      }),
      viteCompression({
        algorithm: "brotliCompress",
        ext: ".br",
        deleteOriginFile: false,
      }),
    ],

    resolve: {
      extensions: [".mjs", ".js", ".ts", ".jsx", ".tsx", ".json"],
      dedupe: ["react", "react-dom"], // Force single copy of React
    },

    server: {
      port: Number(env.VITE_PORT) || 3000,
      strictPort: true,
      open: true,
      fs: {
        allow: [".."],
      },
      hmr: {
        protocol: "ws",
      },
    },

    preview: {
      port: Number(env.VITE_PREVIEW_PORT) || 3000,
      strictPort: true,
    },

    build: {
      outDir: "dist",
      emptyOutDir: true,
      sourcemap: !isProd,
      target: "es2020",
      minify: isProd ? "terser" : false,
      terserOptions: isProd
        ? {
            compress: {
              drop_console: true,
              drop_debugger: true,
            },
            format: {
              comments: false,
            },
          }
        : undefined,
      rollupOptions: {
        output: {
          manualChunks: {
            // Bundle React and all React-related packages together
            "vendor-react": ["react", "react-dom", "react/jsx-runtime"],
          },
        },
      },
      chunkSizeWarningLimit: 2000,
    },

    optimizeDeps: {
      include: ["react", "react-dom", "react/jsx-runtime"],
      esbuildOptions: {
        target: "es2020",
      },
    },

    define: {
      __DEV__: !isProd,
      __APP_ENV__: JSON.stringify(env.APP_ENV ?? mode),
    },

    css: {
      devSourcemap: true,
      postcss: {},
    },

    clearScreen: true,
  };
});

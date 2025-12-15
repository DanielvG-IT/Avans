import js from "@eslint/js";
import path from "node:path";
import globals from "globals";
import prettier from "prettier";
import tseslint from "typescript-eslint";
import { fileURLToPath } from "node:url";
import reactHooks from "eslint-plugin-react-hooks";
import reactRefresh from "eslint-plugin-react-refresh";
import { defineConfig, globalIgnores } from "eslint/config";

// Resolve current file and root
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const rootPrettierConfigPath = path.resolve(__dirname, "../../.prettierrc");

// Load root Prettier config dynamically
const rootPrettierConfig = await prettier.resolveConfig(rootPrettierConfigPath);

export default defineConfig([
  globalIgnores(["dist"]),
  {
    files: ["**/*.{ts,tsx}"],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs["recommended-latest"],
      reactRefresh.configs.vite,
    ],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
      parserOptions: {
        // Disambiguate between multiple tsconfig roots in the monorepo
        tsconfigRootDir: __dirname,
      },
    },
    rules: {
      // Prettier integration: enforce formatting according to root .prettierrc
      "prettier/prettier": ["error", rootPrettierConfig || {}],
    },
    plugins: {
      prettier: require("eslint-plugin-prettier"),
    },
  },
]);

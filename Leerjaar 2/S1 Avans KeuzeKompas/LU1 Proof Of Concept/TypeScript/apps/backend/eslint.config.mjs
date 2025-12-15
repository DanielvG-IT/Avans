import path from "node:path";
import globals from "globals";
import prettier from "prettier";
import tseslint from "typescript-eslint";
import { fileURLToPath } from "node:url";
import eslintPluginPrettierRecommended from "eslint-plugin-prettier/recommended";

// Resolve current file and root
const __dirname = path.dirname(fileURLToPath(import.meta.url));
const rootPrettierConfigPath = path.resolve(__dirname, "../../.prettierrc");

// Load root Prettier config
const rootPrettierConfig = await prettier.resolveConfig(rootPrettierConfigPath);

export default tseslint.config(
  {
    ignores: ["eslint.config.mjs"],
  },
  tseslint.configs.recommendedTypeChecked,
  eslintPluginPrettierRecommended,
  {
    languageOptions: {
      globals: {
        ...globals.node,
        ...globals.jest,
      },
      sourceType: "commonjs",
      parserOptions: {
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
    rules: {
      "@typescript-eslint/no-explicit-any": "off",
      "@typescript-eslint/no-floating-promises": "warn",
      "@typescript-eslint/no-unsafe-argument": "warn",
      // Prettier rule uses root config
      "prettier/prettier": ["error", rootPrettierConfig || {}],
    },
  },
);

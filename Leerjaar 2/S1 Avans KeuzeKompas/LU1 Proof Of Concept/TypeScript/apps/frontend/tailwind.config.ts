/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: "class",
  content: [
    // your template files
    "./src/**/*.{js,ts,jsx,tsx,html}",
  ],
  theme: {
    screens: {
      xs: "475px",
      // => @media (min-width: 475px) { ... }

      sm: "640px",
      // => @media (min-width: 640px) { ... }

      md: "768px",
      // => @media (min-width: 768px) { ... }

      lg: "1024px",
      // => @media (min-width: 1024px) { ... }

      xl: "1280px",
      // => @media (min-width: 1280px) { ... }

      "2xl": "1536px",
      // => @media (min-width: 1536px) { ... }
    },
    extend: {
      colors: {
        avans: {
          red: "#c6002a",
          lightred: "#F15A24",
          dark: "#111111",
          gray: {
            100: "#F7F7F7",
            200: "#E1E1E1",
            300: "#CFCFCF",
            400: "#B1B1B1",
            500: "#9E9E9E",
            600: "#7E7E7E",
            700: "#626262",
            800: "#4B4B4B",
            900: "#333333",
          },
        },
      },
      fontFamily: {
        sans: ["Open Sans", "Helvetica", "Arial", "sans-serif"],
        heading: ["Roboto Slab", "Georgia", "serif"],
      },
      spacing: {
        18: "4.5rem",
        22: "5.5rem",
        26: "6.5rem",
        "safe-top": "env(safe-area-inset-top)",
        "safe-bottom": "env(safe-area-inset-bottom)",
        "safe-left": "env(safe-area-inset-left)",
        "safe-right": "env(safe-area-inset-right)",
      },
      borderRadius: {
        lg: "0.75rem",
      },
      minHeight: {
        touch: "44px", // Minimum touch target size for accessibility
      },
      minWidth: {
        touch: "44px", // Minimum touch target size for accessibility
      },
    },
  },
  variants: {
    extend: {
      backgroundColor: ["active", "hover"],
      textColor: ["active", "hover"],
      borderColor: ["hover", "focus"],
    },
  },
  plugins: [require("@tailwindcss/typography"), require("@tailwindcss/forms")],
};

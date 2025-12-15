import { ThemeProvider as NextThemesProvider } from "next-themes";
import type { ThemeProviderProps } from "next-themes";

/**
 * ThemeProvider component that wraps the app with theme context.
 * Uses next-themes for theme management with localStorage persistence.
 *
 * @param props - Theme provider configuration
 * @param props.children - Child components to wrap
 * @param props.defaultTheme - Default theme if none is saved (default: "system")
 * @param props.storageKey - localStorage key for persisting theme (default: "theme")
 * @param props.attribute - HTML attribute to set for theme (default: "class")
 * @param props.enableSystem - Enable system preference detection (default: true)
 * @param props.disableTransitionOnChange - Disable transitions during theme change (default: false)
 */
export function ThemeProvider({
  children,
  defaultTheme = "system",
  storageKey = "theme",
  attribute = "class",
  enableSystem = true,
  disableTransitionOnChange = false,
  ...props
}: ThemeProviderProps) {
  return (
    <NextThemesProvider
      defaultTheme={defaultTheme}
      storageKey={storageKey}
      attribute={attribute}
      enableSystem={enableSystem}
      disableTransitionOnChange={disableTransitionOnChange}
      {...props}
    >
      {children}
    </NextThemesProvider>
  );
}

import { Moon, Sun, Monitor } from "lucide-react";
import { useTheme } from "next-themes";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

/**
 * ThemeToggle component that allows users to switch between light, dark, and system themes.
 * Displays an accessible select menu with icons and labels for each theme option.
 * Persists user choice in localStorage and respects OS preference when set to "system".
 *
 * Features:
 * - Accessible with proper ARIA labels
 * - Visual icons for each theme mode
 * - Smooth transitions between themes
 * - Hydration-safe (prevents flash of incorrect theme)
 */
export function ThemeToggle() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  // Prevent hydration mismatch by only rendering after mount
  useEffect(() => {
    setMounted(true);
  }, []);

  if (!mounted) {
    // Return a placeholder with same dimensions to prevent layout shift
    return <div className="w-[120px] h-10" aria-hidden="true" />;
  }

  const themeIcons = {
    light: <Sun className="h-4 w-4" aria-hidden="true" />,
    dark: <Moon className="h-4 w-4" aria-hidden="true" />,
    system: <Monitor className="h-4 w-4" aria-hidden="true" />,
  };

  const themeLabels = {
    light: "Light",
    dark: "Dark",
    system: "System",
  };

  return (
    <div className="flex items-center gap-2">
      <Select value={theme} onValueChange={setTheme}>
        <SelectTrigger
          className="w-[120px] bg-background border-border hover:bg-accent hover:text-accent-foreground focus:ring-2 focus:ring-ring focus:ring-offset-2 transition-colors"
          aria-label="Select theme"
        >
          <SelectValue placeholder="Theme" />
        </SelectTrigger>
        <SelectContent className="bg-popover border-border">
          <SelectItem
            value="light"
            className="hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground cursor-pointer"
          >
            <div className="flex items-center gap-2">
              {themeIcons.light}
              <span>{themeLabels.light}</span>
            </div>
          </SelectItem>
          <SelectItem
            value="dark"
            className="hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground cursor-pointer"
          >
            <div className="flex items-center gap-2">
              {themeIcons.dark}
              <span>{themeLabels.dark}</span>
            </div>
          </SelectItem>
          <SelectItem
            value="system"
            className="hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground cursor-pointer"
          >
            <div className="flex items-center gap-2">
              {themeIcons.system}
              <span>{themeLabels.system}</span>
            </div>
          </SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}

/**
 * Simple icon-only theme toggle button that cycles through themes.
 * Useful for compact layouts like mobile headers.
 */
export function ThemeToggleButton() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  if (!mounted) {
    return (
      <Button variant="ghost" size="icon" aria-label="Toggle theme" disabled className="w-10 h-10">
        <Sun className="h-5 w-5" />
      </Button>
    );
  }

  const cycleTheme = () => {
    const themes = ["light", "dark", "system"];
    const currentIndex = themes.indexOf(theme || "system");
    const nextIndex = (currentIndex + 1) % themes.length;
    setTheme(themes[nextIndex]);
  };

  const getCurrentIcon = () => {
    switch (theme) {
      case "light":
        return <Sun className="h-5 w-5" aria-hidden="true" />;
      case "dark":
        return <Moon className="h-5 w-5" aria-hidden="true" />;
      default:
        return <Monitor className="h-5 w-5" aria-hidden="true" />;
    }
  };

  const getAriaLabel = () => {
    switch (theme) {
      case "light":
        return "Switch to dark theme";
      case "dark":
        return "Switch to system theme";
      default:
        return "Switch to light theme";
    }
  };

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={cycleTheme}
      aria-label={getAriaLabel()}
      className="hover:bg-accent hover:text-accent-foreground focus:ring-2 focus:ring-ring focus:ring-offset-2 transition-colors"
    >
      {getCurrentIcon()}
      <span className="sr-only">{getAriaLabel()}</span>
    </Button>
  );
}

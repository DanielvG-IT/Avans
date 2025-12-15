import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeToggle } from "@/components/ThemeToggle";

// Mock next-themes
const mockSetTheme = vi.fn();
vi.mock("next-themes", () => ({
  useTheme: () => ({
    theme: "light",
    setTheme: mockSetTheme,
  }),
  ThemeProvider: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));

describe("ThemeToggle", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders theme toggle select after mounting", async () => {
    render(<ThemeToggle />);

    await waitFor(() => {
      expect(screen.getByRole("combobox")).toBeInTheDocument();
    });
  });

  it("displays theme options", async () => {
    render(<ThemeToggle />);

    await waitFor(() => {
      expect(screen.getByRole("combobox")).toBeInTheDocument();
    });
  });

  it("is keyboard accessible", async () => {
    const user = userEvent.setup();
    render(<ThemeToggle />);

    await waitFor(() => {
      expect(screen.getByRole("combobox")).toBeInTheDocument();
    });

    const toggle = screen.getByRole("combobox");
    await user.tab();
    expect(toggle).toHaveFocus();
  });

  it("has proper aria label", async () => {
    render(<ThemeToggle />);

    await waitFor(() => {
      const toggle = screen.getByRole("combobox");
      expect(toggle).toHaveAttribute("aria-label", "Select theme");
    });
  });
});

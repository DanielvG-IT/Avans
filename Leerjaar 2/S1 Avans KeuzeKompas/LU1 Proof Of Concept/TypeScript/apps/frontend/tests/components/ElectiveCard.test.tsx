import { describe, it, expect, vi } from "vitest";
import { render, screen } from "../helpers/test-utils";
import userEvent from "@testing-library/user-event";
import ElectiveCard from "@/components/elective/ElectiveCard";
import { mockElective } from "../helpers/mockData";

// Mock useNavigate
const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

describe("ElectiveCard", () => {
  it("renders elective information correctly", () => {
    render(<ElectiveCard elective={mockElective} />);

    expect(screen.getByText("Advanced TypeScript")).toBeInTheDocument();
    expect(screen.getByText(/ELEC001/)).toBeInTheDocument();
    expect(screen.getByText(/English/)).toBeInTheDocument();
    expect(
      screen.getByText(
        "Learn advanced TypeScript patterns and best practices for modern web development.",
      ),
    ).toBeInTheDocument();
  });

  it("displays all metadata badges", () => {
    render(<ElectiveCard elective={mockElective} />);

    expect(screen.getByText("P3")).toBeInTheDocument();
    expect(screen.getByText("1 Periode")).toBeInTheDocument();
    expect(screen.getByText("5 EC")).toBeInTheDocument();
    expect(screen.getByText("NLQF5")).toBeInTheDocument();
    expect(screen.getByText("Breda")).toBeInTheDocument();
  });

  it("navigates to elective details when clicked", async () => {
    const user = userEvent.setup();
    render(<ElectiveCard elective={mockElective} />);

    const card = screen.getByRole("article");
    await user.click(card);

    expect(mockNavigate).toHaveBeenCalledWith("/electives/1", {
      replace: false,
      state: { from: "/" },
    });
  });

  it("navigates when Enter key is pressed", async () => {
    const user = userEvent.setup();
    render(<ElectiveCard elective={mockElective} />);

    const card = screen.getByRole("article");
    card.focus();
    await user.keyboard("{Enter}");

    expect(mockNavigate).toHaveBeenCalled();
  });

  it("navigates when Space key is pressed", async () => {
    const user = userEvent.setup();
    render(<ElectiveCard elective={mockElective} />);

    const card = screen.getByRole("article");
    card.focus();
    await user.keyboard(" ");

    expect(mockNavigate).toHaveBeenCalled();
  });

  it("shows fallback description when description is empty", () => {
    const electiveWithoutDesc = {
      ...mockElective,
      description: "",
    };
    render(<ElectiveCard elective={electiveWithoutDesc} />);

    expect(screen.getByText("No description available.")).toBeInTheDocument();
  });

  it("is keyboard accessible", () => {
    render(<ElectiveCard elective={mockElective} />);

    const card = screen.getByRole("article");
    expect(card).toHaveAttribute("tabIndex", "0");
  });

  it("does not display metadata for null values", () => {
    const electiveWithNullCredits = {
      ...mockElective,
      credits: null as any,
      level: null as any,
      location: null as any,
      period: null as any,
      duration: null as any,
    };
    render(<ElectiveCard elective={electiveWithNullCredits} />);

    // Should not show credits badge (use more specific pattern)
    expect(screen.queryByText("5 EC")).not.toBeInTheDocument();
    // Should not show level badge
    expect(screen.queryByText("NLQF5")).not.toBeInTheDocument();
  });
});

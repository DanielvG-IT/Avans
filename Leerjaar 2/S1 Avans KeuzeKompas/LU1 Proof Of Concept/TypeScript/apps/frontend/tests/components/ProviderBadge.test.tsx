import { describe, it, expect } from "vitest";
import { render, screen } from "../helpers/test-utils";
import ProviderBadge from "@/components/elective/ProviderBadge";

describe("ProviderBadge", () => {
  it("renders provider name correctly", () => {
    render(<ProviderBadge provider="Technische Bedrijfskunde" />);

    expect(screen.getByText("Technische Bedrijfskunde")).toBeInTheDocument();
  });

  it("renders N/A when codeOnly is true but no code in parentheses", () => {
    render(<ProviderBadge provider="Technische Bedrijfskunde" codeOnly={true} />);

    expect(screen.getByText("N/A")).toBeInTheDocument();
  });

  it("extracts code from parentheses when present", () => {
    render(<ProviderBadge provider="Informatica (INF)" codeOnly={true} />);

    expect(screen.getByText("(INF)")).toBeInTheDocument();
  });

  it("handles provider with code in format", () => {
    render(<ProviderBadge provider="Software Engineering (SE)" codeOnly={true} />);

    expect(screen.getByText("(SE)")).toBeInTheDocument();
  });

  it("displays full name for unknown provider when codeOnly is false", () => {
    render(<ProviderBadge provider="Unknown Department" />);

    expect(screen.getByText("Unknown Department")).toBeInTheDocument();
  });

  it("displays N/A for provider without code when codeOnly is true", () => {
    render(<ProviderBadge provider="Unknown Department" codeOnly={true} />);

    expect(screen.getByText("N/A")).toBeInTheDocument();
  });

  it("renders badge with proper structure", () => {
    render(<ProviderBadge provider="Test Provider" />);

    // Check that the badge text is rendered
    expect(screen.getByText("Test Provider")).toBeInTheDocument();
  });

  it("handles undefined provider", () => {
    render(<ProviderBadge provider={undefined} />);

    expect(screen.getByText("Unknown Provider")).toBeInTheDocument();
  });
});

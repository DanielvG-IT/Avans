import { describe, it, expect } from "vitest";
import { render, screen } from "../helpers/test-utils";
import Loading from "@/components/Loading";

describe("Loading", () => {
  it("renders with default props", () => {
    render(<Loading />);

    expect(screen.getByText("Loading…")).toBeInTheDocument();
  });

  it("displays custom message", () => {
    render(<Loading message="Please wait while we fetch your data" />);

    expect(screen.getByText("Please wait while we fetch your data")).toBeInTheDocument();
  });

  it("renders logo when showLogo is true", () => {
    render(<Loading showLogo={true} />);

    const logo = screen.getByAltText("Avans Keuzekompas Logo");
    expect(logo).toBeInTheDocument();
  });

  it("does not render logo when showLogo is false", () => {
    render(<Loading showLogo={false} />);

    const logo = screen.queryByAltText("Keuzekompas Logo");
    expect(logo).not.toBeInTheDocument();
  });

  it("applies custom className", () => {
    const { container } = render(<Loading className="custom-loading-class" />);

    expect(container.firstChild).toHaveClass("custom-loading-class");
  });

  it("renders full screen when isFullScreen is true", () => {
    const { container } = render(<Loading isFullScreen={true} />);

    const loadingContainer = container.firstChild as HTMLElement;
    expect(loadingContainer).toHaveStyle({ height: "100vh" });
  });

  it("renders small size correctly", () => {
    render(<Loading size="small" />);

    // The component should render, specific size styling is applied via inline styles
    expect(screen.getByText("Loading…")).toBeInTheDocument();
  });

  it("renders medium size correctly", () => {
    render(<Loading size="medium" />);

    expect(screen.getByText("Loading…")).toBeInTheDocument();
  });

  it("renders large size correctly", () => {
    render(<Loading size="large" />);

    expect(screen.getByText("Loading…")).toBeInTheDocument();
  });
});

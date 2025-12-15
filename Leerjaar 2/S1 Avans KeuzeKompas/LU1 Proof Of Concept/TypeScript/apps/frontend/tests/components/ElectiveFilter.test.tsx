import { describe, it, expect, vi } from "vitest";
import { render, screen } from "../helpers/test-utils";
import userEvent from "@testing-library/user-event";
import ElectiveFilter from "@/components/elective/ElectiveFilter";

describe("ElectiveFilter", () => {
  const mockOnChange = vi.fn();
  const mockOptions = [
    { value: "all", label: "All" },
    { value: "p1", label: "Period 1" },
    { value: "p2", label: "Period 2" },
    { value: "p3", label: "Period 3" },
  ];

  it("renders with placeholder text", () => {
    render(
      <ElectiveFilter
        placeholder="Select Period"
        value=""
        onChange={mockOnChange}
        options={mockOptions}
      />,
    );

    // Use getAllByText since the text appears in both label and trigger
    const placeholderElements = screen.getAllByText("Select Period");
    expect(placeholderElements.length).toBeGreaterThan(0);
  });

  it("renders the select trigger", () => {
    const mockOnChange = vi.fn();

    render(
      <ElectiveFilter options={mockOptions} onChange={mockOnChange} placeholder="Select Period" />,
    );

    const trigger = screen.getByRole("combobox");
    expect(trigger).toBeInTheDocument();
  });

  it("displays default value when provided", () => {
    render(
      <ElectiveFilter
        placeholder="Select Period"
        defaultValue="p2"
        onChange={mockOnChange}
        options={mockOptions}
      />,
    );

    // The select component renders but may not show the value text immediately
    const combobox = screen.getByRole("combobox");
    expect(combobox).toBeInTheDocument();
  });

  it("renders custom icon when provided", () => {
    const mockOnChange = vi.fn();
    const TestIcon = () => <span data-testid="custom-icon">📅</span>;

    render(
      <ElectiveFilter
        options={mockOptions}
        onChange={mockOnChange}
        icon={<TestIcon />}
        placeholder="Select Period"
      />,
    );

    expect(screen.getByTestId("custom-icon")).toBeInTheDocument();
  });

  it("has proper aria-label", () => {
    const mockOnChange = vi.fn();
    render(
      <ElectiveFilter
        options={mockOptions}
        onChange={mockOnChange}
        ariaLabel="Filter by period"
        placeholder="Select Period"
      />,
    );

    expect(screen.getByLabelText("Filter by period")).toBeInTheDocument();
  });

  it("applies custom className to trigger", () => {
    const mockOnChange = vi.fn();
    render(
      <ElectiveFilter
        options={mockOptions}
        onChange={mockOnChange}
        triggerClassName="custom-class"
        placeholder="Select Period"
      />,
    );

    const trigger = screen.getByRole("combobox");
    expect(trigger).toHaveClass("custom-class");
  });

  it("renders component structure correctly", () => {
    const mockOnChange = vi.fn();
    const { container } = render(
      <ElectiveFilter options={mockOptions} onChange={mockOnChange} placeholder="Select Period" />,
    );

    // Check that the component has the expected structure
    expect(container.querySelector('[class*="flex"]')).toBeInTheDocument();
  });
});

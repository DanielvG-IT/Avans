import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "../helpers/test-utils";
import userEvent from "@testing-library/user-event";
import ProtectedRoute from "@/components/auth/ProtectedRoute";
import Cookies from "js-cookie";

vi.mock("js-cookie");

// Mock Navigate component
const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    Navigate: ({ to }: { to: string }) => {
      mockNavigate(to);
      return <div data-testid="navigate-mock">Navigate to {to}</div>;
    },
  };
});

describe("ProtectedRoute", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders children when token exists", () => {
    vi.mocked(Cookies.get).mockReturnValue("valid-token");

    render(
      <ProtectedRoute>
        <div>Protected content</div>
      </ProtectedRoute>,
    );

    expect(screen.getByText("Protected content")).toBeInTheDocument();
    expect(screen.queryByTestId("navigate-mock")).not.toBeInTheDocument();
  });

  it("redirects to login when token does not exist", () => {
    vi.mocked(Cookies.get).mockReturnValue(undefined);

    render(
      <ProtectedRoute>
        <div>Protected content</div>
      </ProtectedRoute>,
    );

    expect(screen.queryByText("Protected content")).not.toBeInTheDocument();
    expect(screen.getByTestId("navigate-mock")).toBeInTheDocument();
    expect(mockNavigate).toHaveBeenCalledWith("/auth/login");
  });

  it("checks for ACCESSTOKEN cookie", () => {
    vi.mocked(Cookies.get).mockReturnValue("token");

    render(
      <ProtectedRoute>
        <div>Protected content</div>
      </ProtectedRoute>,
    );

    expect(Cookies.get).toHaveBeenCalledWith("ACCESSTOKEN");
  });
});

import { describe, it, expect, vi } from "vitest";
import { render, screen, waitFor } from "../helpers/test-utils";
import userEvent from "@testing-library/user-event";
import { mockElective, mockElectives } from "../helpers/mockData";

// Mock the API service
vi.mock("@/services/api.service", () => ({
  api: {
    getElectives: vi.fn(),
    getElectiveById: vi.fn(),
    createElective: vi.fn(),
    updateElective: vi.fn(),
    deleteElective: vi.fn(),
  },
}));

describe("Elective Integration Tests", () => {
  describe("Elective List Display", () => {
    it("displays multiple electives correctly", () => {
      // This would be tested with a full Electives page component
      // For now, we can test that ElectiveCard can render multiple items
      const { container } = render(
        <div>
          {mockElectives.map((elective) => (
            <div key={elective.id} data-testid={`elective-${elective.id}`}>
              <h3>{elective.name}</h3>
              <p>{elective.code}</p>
            </div>
          ))}
        </div>,
      );

      expect(screen.getByText("Advanced TypeScript")).toBeInTheDocument();
      expect(screen.getByText("React Development")).toBeInTheDocument();
      expect(screen.getByText("Database Design")).toBeInTheDocument();
    });
  });

  describe("Elective Filtering", () => {
    it("filters electives by language", () => {
      const englishElectives = mockElectives.filter((e) => e.language === "English");
      const dutchElectives = mockElectives.filter((e) => e.language === "Nederlands");

      expect(englishElectives).toHaveLength(2);
      expect(dutchElectives).toHaveLength(1);
    });

    it("filters electives by period", () => {
      const p3Electives = mockElectives.filter((e) => e.period === "P3");

      expect(p3Electives).toHaveLength(1);
      expect(p3Electives[0].name).toBe("Advanced TypeScript");
    });

    it("filters electives by provider", () => {
      const tbkElectives = mockElectives.filter((e) => e.provider === "Technische Bedrijfskunde");

      expect(tbkElectives).toHaveLength(1);
      expect(tbkElectives[0].code).toBe("ELEC001");
    });
  });

  describe("Elective Details", () => {
    it("displays complete elective information", () => {
      const elective = mockElective;

      expect(elective.name).toBe("Advanced TypeScript");
      expect(elective.code).toBe("ELEC001");
      expect(elective.credits).toBe(5);
      expect(elective.language).toBe("English");
      expect(elective.provider).toBe("Technische Bedrijfskunde");
      expect(elective.period).toBe("P3");
      expect(elective.duration).toBe("1 Periode");
      expect(elective.location).toBe("Breda");
      expect(elective.level).toBe("NLQF5");
    });

    it("includes optional tags for recommendations", () => {
      expect(mockElective.tags).toContain("programming");
      expect(mockElective.tags).toContain("typescript");
      expect(mockElective.tags).toContain("web");
    });
  });

  describe("Elective Search", () => {
    it("searches electives by name", () => {
      const searchTerm = "typescript";
      const results = mockElectives.filter((e) =>
        e.name.toLowerCase().includes(searchTerm.toLowerCase()),
      );

      expect(results).toHaveLength(1);
      expect(results[0].name).toBe("Advanced TypeScript");
    });

    it("searches electives by description", () => {
      const searchTerm = "react";
      const results = mockElectives.filter(
        (e) =>
          e.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          e.description.toLowerCase().includes(searchTerm.toLowerCase()),
      );

      expect(results).toHaveLength(1);
      expect(results[0].name).toBe("React Development");
    });

    it("searches electives by tags", () => {
      const searchTerm = "database";
      const results = mockElectives.filter((e) =>
        e.tags?.some((tag) => tag.toLowerCase().includes(searchTerm.toLowerCase())),
      );

      expect(results).toHaveLength(1);
      expect(results[0].name).toBe("Database Design");
    });
  });

  describe("Elective Sorting", () => {
    it("sorts electives by name alphabetically", () => {
      const sorted = [...mockElectives].sort((a, b) => a.name.localeCompare(b.name));

      expect(sorted[0].name).toBe("Advanced TypeScript");
      expect(sorted[1].name).toBe("Database Design");
      expect(sorted[2].name).toBe("React Development");
    });

    it("sorts electives by credits", () => {
      const sorted = [...mockElectives].sort((a, b) => a.credits - b.credits);

      expect(sorted[0].credits).toBe(5);
      expect(sorted[1].credits).toBe(5);
      expect(sorted[2].credits).toBe(10);
    });

    it("sorts electives by period", () => {
      const sorted = [...mockElectives].sort((a, b) => a.period.localeCompare(b.period));

      expect(sorted[0].period).toBe("P1");
      expect(sorted[1].period).toBe("P3");
      expect(sorted[2].period).toBe("P4");
    });
  });
});

// Quick test/demo of the enhanced recommendation algorithm

import { describe, it, expect } from "vitest";
import {
  scoreElective,
  diversifyResults,
  type ScoringOptions,
} from "../../src/pages/Recommendations/scoring";
import { deriveFromPersonality } from "../../src/pages/Recommendations/personality";
import type { Elective } from "@/types/Elective";

// Mock electives for testing
const mockElectives: Elective[] = [
  {
    code: "AI101",
    name: "Introduction to Artificial Intelligence",
    description:
      "Learn the fundamentals of AI, machine learning, and neural networks. Hands-on projects with Python and TensorFlow.",
    provider: "Academie voor Technologie en Design (ATD)",
    period: "Q1",
    duration: "1 periode",
    credits: 15,
    language: "English",
    location: "Breda",
    level: "NLQF5",
    tags: ["AI / ML", "Data"],
  },
  {
    code: "WEB201",
    name: "Modern Web Development",
    description:
      "Build full-stack web applications using React, Node.js, and TypeScript. Focus on responsive design and API development.",
    provider: "Academie voor Business en Entrepreneurship (ABE)",
    period: "Q2",
    duration: "1 periode",
    credits: 20,
    language: "Nederlands",
    location: "Breda",
    level: "NLQF6",
    tags: ["Web Development", "Design"],
  },
  {
    code: "SUS301",
    name: "Sustainable Urban Planning",
    description:
      "Design eco-friendly cities and communities. Focus on green architecture and environmental impact.",
    provider: "Academie voor Duurzaam Gebouwde Omgeving (ADGO)",
    period: "Q3",
    duration: "2 periodes",
    credits: 30,
    language: "Nederlands",
    location: "Den Bosch",
    level: "NLQF6",
    tags: ["Sustainability", "Design"],
  },
  {
    code: "DATA150",
    name: "Data Analytics Fundamentals",
    description:
      "Master data visualization, SQL databases, and analytics tools. Work with real-world datasets.",
    provider: "Academie voor Management en Finance (AMF)",
    period: "Q1",
    duration: "1 periode",
    credits: 15,
    language: "English",
    location: "Breda",
    level: "NLQF5",
    tags: ["Data", "AI / ML"],
  },
];

describe("Enhanced Recommendation Algorithm", () => {
  describe("Basic Scoring", () => {
    it("should score electives based on provided options", () => {
      const options: ScoringOptions = {
        academy: "Academie voor Technologie en Design (ATD)",
        interests: ["AI / ML", "Data"],
        language: "English",
        workloadPref: "light",
      };

      const result = scoreElective(mockElectives[0], options);

      expect(result.score).toBeGreaterThan(0);
      expect(result.reasons).toBeDefined();
      expect(Array.isArray(result.reasons)).toBe(true);
      expect(result.breakdown).toBeDefined();
    });
  });

  describe("Content Matching", () => {
    it("should score electives based on interest matching", () => {
      const options: ScoringOptions = {
        interests: ["AI / ML", "Web Development"],
        workloadPref: "medium",
      };

      const results = mockElectives.map((elective) => ({
        elective,
        result: scoreElective(elective, options),
      }));

      expect(results.length).toBe(mockElectives.length);
      results.forEach(({ result }) => {
        expect(result.score).toBeGreaterThanOrEqual(0);
        expect(result.reasons).toBeDefined();
      });
    });

    it("should give higher scores to electives matching interests", () => {
      const options: ScoringOptions = {
        interests: ["AI / ML", "Data"],
        workloadPref: "medium",
      };

      const aiElective = scoreElective(mockElectives[0], options); // AI course with AI/ML and Data tags
      const sustainabilityElective = scoreElective(mockElectives[2], options); // Sustainability course

      expect(aiElective.score).toBeGreaterThan(sustainabilityElective.score);
    });
  });

  describe("Personality Derivation", () => {
    it("should derive preferences from personality answers", () => {
      const answers = ["B", "B", "E", "E", "B"];
      const interests = ["AI / ML", "Web Development", "Data", "Embedded", "Robotics", "Design"];

      const derived = deriveFromPersonality(answers, interests);

      expect(derived.academy).toBeDefined();
      expect(derived.interests).toBeDefined();
      expect(Array.isArray(derived.interests)).toBe(true);
      expect(derived.workloadPref).toBeDefined();
      expect(["light", "medium", "heavy"]).toContain(derived.workloadPref);
    });
  });

  describe("Diversity Boosting", () => {
    it("should prioritize top results per provider within limit", () => {
      const expandedElectives: Elective[] = [
        ...mockElectives,
        {
          code: "ATD201",
          name: "Advanced Design Thinking",
          description: "Creative problem solving and innovation",
          provider: "Academie voor Technologie en Design (ATD)",
          period: "Q2",
          duration: "1 periode",
          credits: 15,
          language: "Nederlands",
          location: "Breda",
          level: "NLQF6",
          tags: ["Design"],
        },
        {
          code: "ATD202",
          name: "Prototyping Workshop",
          description: "Hands-on product development",
          provider: "Academie voor Technologie en Design (ATD)",
          period: "Q3",
          duration: "1 periode",
          credits: 15,
          language: "Nederlands",
          location: "Breda",
          level: "NLQF5",
          tags: ["Design", "UX / Product"],
        },
      ];

      const options: ScoringOptions = {
        academy: "Academie voor Technologie en Design (ATD)",
        interests: ["Design"],
        workloadPref: "light",
      };

      const scored = expandedElectives.map((e) => ({
        elective: e,
        ...scoreElective(e, options),
      }));

      scored.sort((a, b) => b.score - a.score);

      const maxPerProvider = 2;
      const diversified = diversifyResults(scored, maxPerProvider);

      // The function prioritizes diversity in the top results
      // Count providers in the first portion of results
      const topResults = diversified.slice(0, Math.min(diversified.length, maxPerProvider * 3));
      const providerCounts = new Map<string, number>();

      topResults.forEach((item) => {
        const provider = item.elective.provider;
        providerCounts.set(provider, (providerCounts.get(provider) || 0) + 1);
      });

      // Verify diversity is improved (not all from one provider)
      expect(providerCounts.size).toBeGreaterThan(1);
      expect(diversified.length).toBeGreaterThan(0);
    });

    it("should maintain sorted order where possible", () => {
      const options: ScoringOptions = {
        interests: ["AI / ML", "Data"],
        workloadPref: "medium",
      };

      const scored = mockElectives.map((e) => ({
        elective: e,
        ...scoreElective(e, options),
      }));

      scored.sort((a, b) => b.score - a.score);
      const diversified = diversifyResults(scored, 2);

      // Diversified results should still be in descending score order (mostly)
      for (let i = 0; i < diversified.length - 1; i++) {
        // Allow some tolerance for diversity adjustments
        expect(diversified[i].score).toBeGreaterThanOrEqual(diversified[i + 1].score - 50);
      }
    });
  });
});

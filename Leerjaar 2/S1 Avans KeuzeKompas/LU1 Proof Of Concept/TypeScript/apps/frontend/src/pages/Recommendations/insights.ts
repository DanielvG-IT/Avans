// Insights and analytics for recommendation results

import type { Elective } from "@/types/Elective";
import type { ScoringResult } from "./scoring";

export interface RecommendationInsights {
  topProvider: string;
  topInterests: string[];
  avgScore: number;
  scoreDistribution: {
    high: number; // 80-100
    medium: number; // 50-79
    low: number; // 0-49
  };
  recommendations: {
    byProvider: Record<string, number>;
    byLanguage: Record<string, number>;
    byCredits: Record<string, number>;
  };
}

export function generateInsights(
  results: Array<{ elective: Elective; score: number; reasons: string[] }>,
): RecommendationInsights {
  if (results.length === 0) {
    return {
      topProvider: "N/A",
      topInterests: [],
      avgScore: 0,
      scoreDistribution: { high: 0, medium: 0, low: 0 },
      recommendations: {
        byProvider: {},
        byLanguage: {},
        byCredits: {},
      },
    };
  }

  // Provider distribution
  const providerCounts: Record<string, number> = {};
  results.forEach(({ elective }) => {
    const provider = elective.provider || "Unknown";
    providerCounts[provider] = (providerCounts[provider] || 0) + 1;
  });

  const topProvider = Object.entries(providerCounts).sort((a, b) => b[1] - a[1])[0]?.[0] || "N/A";

  // Interest extraction from tags
  const interestCounts: Record<string, number> = {};
  results.forEach(({ elective }) => {
    elective.tags?.forEach((tag) => {
      interestCounts[tag] = (interestCounts[tag] || 0) + 1;
    });
  });

  const topInterests = Object.entries(interestCounts)
    .sort((a, b) => b[1] - a[1])
    .slice(0, 5)
    .map(([interest]) => interest);

  // Score statistics
  const scores = results.map((r) => r.score);
  const avgScore = scores.reduce((sum, s) => sum + s, 0) / scores.length;

  const scoreDistribution = {
    high: scores.filter((s) => s >= 80).length,
    medium: scores.filter((s) => s >= 50 && s < 80).length,
    low: scores.filter((s) => s < 50).length,
  };

  // Language distribution
  const languageCounts: Record<string, number> = {};
  results.forEach(({ elective }) => {
    const lang = elective.language || "Unknown";
    languageCounts[lang] = (languageCounts[lang] || 0) + 1;
  });

  // Credits distribution
  const creditCounts: Record<string, number> = {};
  results.forEach(({ elective }) => {
    const credits = elective.credits || 0;
    const bucket =
      credits <= 15
        ? "Light (≤15 EC)"
        : credits <= 20
          ? "Medium (15-20 EC)"
          : credits <= 30
            ? "Heavy (20-30 EC)"
            : "Very Heavy (>30 EC)";
    creditCounts[bucket] = (creditCounts[bucket] || 0) + 1;
  });

  return {
    topProvider,
    topInterests,
    avgScore: Math.round(avgScore),
    scoreDistribution,
    recommendations: {
      byProvider: providerCounts,
      byLanguage: languageCounts,
      byCredits: creditCounts,
    },
  };
}

// Helper to explain score breakdown
export function explainScore(breakdown: ScoringResult["breakdown"]): string {
  const parts: string[] = [];

  if (breakdown.academy > 0) {
    parts.push(`Academy match: ${breakdown.academy}pts`);
  }
  if (breakdown.interests > 0) {
    parts.push(`Interests: ${breakdown.interests}pts`);
  }
  if (breakdown.content > 0) {
    parts.push(`Content relevance: ${breakdown.content}pts`);
  }
  if (breakdown.language > 0) {
    parts.push(`Language: ${breakdown.language}pts`);
  }
  if (breakdown.workload > 0) {
    parts.push(`Workload fit: ${breakdown.workload}pts`);
  }
  if (breakdown.quality !== 0) {
    parts.push(`Quality: ${breakdown.quality > 0 ? "+" : ""}${breakdown.quality}pts`);
  }

  return parts.join(" • ");
}

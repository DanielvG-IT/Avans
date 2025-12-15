// Enhanced scoring logic for electives based on user preferences

import type { Elective } from "@/types/Elective";

export interface ScoringOptions {
  academy?: string | undefined;
  interests: string[];
  language?: string | undefined;
  workloadPref?: "light" | "medium" | "heavy" | undefined;
  location?: string | undefined;
  period?: string | undefined;
}

export interface ScoringResult {
  score: number;
  reasons: string[];
  breakdown: {
    academy: number;
    interests: number;
    language: number;
    workload: number;
    content: number;
    quality: number;
  };
}

// Interest keyword mapping for fuzzy matching
const INTEREST_KEYWORDS: Record<string, string[]> = {
  "ai / ml": [
    "ai",
    "ml",
    "machine learning",
    "artificial intelligence",
    "neural",
    "deep learning",
    "nlp",
  ],
  "web development": [
    "web",
    "frontend",
    "backend",
    "fullstack",
    "react",
    "angular",
    "vue",
    "javascript",
    "typescript",
  ],
  data: ["data", "analytics", "database", "sql", "visualization", "big data", "datawarehouse"],
  embedded: ["embedded", "iot", "microcontroller", "arduino", "raspberry", "hardware", "firmware"],
  security: ["security", "cybersecurity", "encryption", "penetration", "firewall", "vulnerability"],
  robotics: ["robot", "automation", "mechatronics", "sensor", "actuator"],
  design: ["design", "ui", "ux", "user experience", "interface", "visual", "graphic"],
  projectmanagement: ["project", "management", "agile", "scrum", "planning", "coordination"],
  sustainability: [
    "sustainability",
    "sustainable",
    "green",
    "environment",
    "climate",
    "eco",
    "renewable",
  ],
  "ux / product": [
    "ux",
    "product",
    "user experience",
    "usability",
    "human-centered",
    "interaction",
  ],
};

// Helper: Check if text contains any of the keywords
function containsKeywords(text: string, keywords: string[]): boolean {
  const lower = text.toLowerCase();
  return keywords.some((kw) => lower.includes(kw));
}

// Helper: Calculate content relevance score
function analyzeContent(
  description: string,
  interests: string[],
): { score: number; matches: string[] } {
  let score = 0;
  const matches: string[] = [];

  interests.forEach((interest) => {
    const keywords = INTEREST_KEYWORDS[interest.toLowerCase()] || [interest.toLowerCase()];
    if (containsKeywords(description, keywords)) {
      score += 10;
      matches.push(interest);
    }
  });

  return { score: Math.min(score, 30), matches };
}

// Helper: Calculate workload match score with better granularity
function calculateWorkloadScore(credits: number, pref: "light" | "medium" | "heavy"): number {
  const thresholds = {
    light: { min: 0, ideal: 15, max: 20 },
    medium: { min: 15, ideal: 20, max: 25 },
    heavy: { min: 25, ideal: 30, max: 50 },
  };

  const range = thresholds[pref];

  // Perfect match
  if (credits === range.ideal) return 15;

  // Good match within range
  if (credits >= range.min && credits <= range.max) {
    const distance = Math.abs(credits - range.ideal);
    return Math.max(5, 15 - distance);
  }

  // Slight penalty for outside preferred range
  return 0;
}

// Helper: Calculate quality indicators
function calculateQualityScore(e: Elective): number {
  let score = 0;

  // Well-documented courses (good description)
  if (e.description && e.description.length >= 200 && e.description.length <= 1000) {
    score += 5;
  }

  // Has tags (better categorized)
  if (e.tags && e.tags.length > 0) {
    score += Math.min(3, e.tags.length);
  }

  // Has teachers assigned
  if (e.teachers && e.teachers.length > 0) {
    score += 2;
  }

  // Penalty for very sparse information
  if (!e.description || e.description.length < 100) {
    score -= 3;
  }

  return score;
}

export function scoreElective(e: Elective, opts: ScoringOptions): ScoringResult {
  let academyScore = 0;
  let interestsScore = 0;
  let languageScore = 0;
  let workloadScore = 0;
  let contentScore = 0;

  const reasons: string[] = [];

  // 1. Academy/Provider Match (Weight: 30-35 points)
  if (opts.academy && e.provider) {
    const academyNormalized = opts.academy.toLowerCase();
    const providerNormalized = e.provider.toLowerCase();

    if (academyNormalized === providerNormalized) {
      academyScore = 35;
      reasons.push("✓ Exacte match met academie");
    } else if (
      providerNormalized.includes(academyNormalized) ||
      academyNormalized.includes(providerNormalized)
    ) {
      academyScore = 30;
      reasons.push("✓ Past bij gekozen academie");
    }
  }

  // 2. Tags/Interests Match (Weight: 0-35 points)
  const tags = (e.tags ?? []).map((t) => t.toLowerCase());
  const interestMatches = opts.interests
    .map((i) => i.toLowerCase())
    .filter((i) => tags.includes(i));

  if (interestMatches.length > 0) {
    // Progressive scoring: first match worth more
    interestsScore = Math.min(15 + (interestMatches.length - 1) * 10, 35);
    reasons.push(
      `✓ ${interestMatches.length} interesse${interestMatches.length > 1 ? "s" : ""} match`,
    );
  }

  // 3. Content Analysis (Weight: 0-20 points)
  if (e.description && opts.interests.length > 0) {
    const contentAnalysis = analyzeContent(e.description, opts.interests);
    contentScore = contentAnalysis.score;

    if (contentAnalysis.matches.length > 0 && interestMatches.length === 0) {
      reasons.push(`✓ Relevante inhoud: ${contentAnalysis.matches.join(", ")}`);
    }
  }

  // 4. Language Preference (Weight: 0-10 points)
  if (opts.language && e.language) {
    if (e.language.toLowerCase().includes(opts.language.toLowerCase())) {
      languageScore = 10;
      reasons.push("✓ Voorkeurstaal");
    }
  }

  // 5. Workload Preference (Weight: 0-15 points)
  if (opts.workloadPref && typeof e.credits === "number") {
    workloadScore = calculateWorkloadScore(e.credits, opts.workloadPref);

    if (workloadScore >= 10) {
      const labels = { light: "Lichte", medium: "Gemiddelde", heavy: "Zware" };
      reasons.push(`✓ ${labels[opts.workloadPref]} workload (${e.credits} EC)`);
    }
  }

  // 6. Location Match (Weight: 0-5 points)
  if (opts.location && e.location) {
    if (e.location.toLowerCase().includes(opts.location.toLowerCase())) {
      reasons.push(`✓ Locatie: ${e.location}`);
    }
  }

  // 7. Quality Indicators (Weight: -3 to +10 points)
  const qualityScore = calculateQualityScore(e);

  // Calculate total score
  const totalScore =
    academyScore + interestsScore + contentScore + languageScore + workloadScore + qualityScore;

  // Normalize to 0-100 range
  const normalized = Math.max(0, Math.min(100, Math.round(totalScore)));

  return {
    score: normalized,
    reasons: reasons.length > 0 ? reasons : ["Beschikbare module"],
    breakdown: {
      academy: academyScore,
      interests: interestsScore,
      language: languageScore,
      workload: workloadScore,
      content: contentScore,
      quality: qualityScore,
    },
  };
}

// Diversity boosting: ensure variety in results
export function diversifyResults(
  scored: Array<{ elective: Elective; score: number; reasons: string[] }>,
  maxPerProvider: number = 3,
): Array<{ elective: Elective; score: number; reasons: string[] }> {
  const providerCounts: Record<string, number> = {};
  const diversified: typeof scored = [];
  const deferred: typeof scored = [];

  scored.forEach((item) => {
    const provider = item.elective.provider || "Unknown";
    const count = providerCounts[provider] || 0;

    if (count < maxPerProvider) {
      diversified.push(item);
      providerCounts[provider] = count + 1;
    } else {
      deferred.push(item);
    }
  });

  // Add deferred items if we still have space
  return [...diversified, ...deferred];
}

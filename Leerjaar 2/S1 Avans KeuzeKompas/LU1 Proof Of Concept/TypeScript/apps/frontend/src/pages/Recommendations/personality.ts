// Enhanced personality-based preference derivation logic

export interface DerivedPreferences {
  academy?: string;
  interests: string[];
  workloadPref: "light" | "medium" | "heavy";
  language?: string;
}

// Academy name mapping for consistency
const ACADEMY_NAMES: Record<string, string> = {
  AWG: "Academie voor Welzijn en Gezondheid (AWG)",
  AWEG: "Academie voor Welzijn Educatie en Gezondheid (AWEG)",
  ALST: "Academie voor Life Sciences en Technologie (ALST)",
  ATD: "Academie voor Technologie en Design (ATD)",
  AACI: "Avans Academie Creative Innovation (AACI)",
  ABE: "Academie voor Business en Entrepreneurship (ABE)",
  AMF: "Academie voor Management en Finance (AMF)",
  ADGO: "Academie voor Duurzaam Gebouwde Omgeving (ADGO)",
  ATIx: "Academie voor Technologie en Innovatie x (ATIx)",
  AWO: "Academie voor Waardecreatie en Ondernemerschap (AWO)",
  AMBF: "Academie voor Management Bestuur en Finance (AMBF)",
};

export function deriveFromPersonality(
  answers: (string | null)[],
  interests: string[],
): DerivedPreferences {
  const interestCandidates: string[] = [];
  const academyVotes: Record<string, number> = {};
  let workloadScore = 0; // negative -> light, positive -> heavy

  // Enhanced mapping rules per answer with weighted scoring
  const mapAnswer = (qIdx: number, answer: string) => {
    switch (qIdx) {
      case 0: // impact - What kind of impact do you want to make?
        if (answer === "A") {
          // Health and well-being
          interestCandidates.push("Sustainability", "Data", "UX / Product");
          academyVotes["AWG"] = (academyVotes["AWG"] ?? 0) + 3;
          academyVotes["AWEG"] = (academyVotes["AWEG"] ?? 0) + 2;
          academyVotes["ALST"] = (academyVotes["ALST"] ?? 0) + 1;
        }
        if (answer === "B") {
          // Create innovative products/designs
          interestCandidates.push("Design", "Embedded", "Robotics", "UX / Product");
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 3;
          academyVotes["AACI"] = (academyVotes["AACI"] ?? 0) + 2;
          academyVotes["ATIx"] = (academyVotes["ATIx"] ?? 0) + 1;
          workloadScore += 1;
        }
        if (answer === "C") {
          // Help businesses grow
          interestCandidates.push("Projectmanagement", "Web Development", "Data");
          academyVotes["ABE"] = (academyVotes["ABE"] ?? 0) + 3;
          academyVotes["AMF"] = (academyVotes["AMF"] ?? 0) + 2;
          academyVotes["AWO"] = (academyVotes["AWO"] ?? 0) + 1;
          workloadScore += 1;
        }
        if (answer === "D") {
          // Sustainable environments
          interestCandidates.push("Sustainability", "Design", "Projectmanagement");
          academyVotes["ADGO"] = (academyVotes["ADGO"] ?? 0) + 3;
          academyVotes["ALST"] = (academyVotes["ALST"] ?? 0) + 1;
        }
        if (answer === "E") {
          // Education and creativity
          interestCandidates.push("Design", "UX / Product", "Web Development");
          academyVotes["AACI"] = (academyVotes["AACI"] ?? 0) + 3;
          academyVotes["AWEG"] = (academyVotes["AWEG"] ?? 0) + 2;
        }
        break;

      case 1: // projects - Which type of project excites you?
        if (answer === "A") {
          // Wellness program
          interestCandidates.push("Sustainability", "Projectmanagement", "UX / Product");
          academyVotes["AWG"] = (academyVotes["AWG"] ?? 0) + 3;
          academyVotes["AWEG"] = (academyVotes["AWEG"] ?? 0) + 2;
          workloadScore -= 1;
        }
        if (answer === "B") {
          // Smart home prototype
          interestCandidates.push("Embedded", "AI / ML", "Robotics", "Web Development");
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 3;
          academyVotes["ATIx"] = (academyVotes["ATIx"] ?? 0) + 2;
          workloadScore += 1;
        }
        if (answer === "C") {
          // Startup/enterprise
          interestCandidates.push("Projectmanagement", "Web Development", "Data");
          academyVotes["ABE"] = (academyVotes["ABE"] ?? 0) + 3;
          academyVotes["AWO"] = (academyVotes["AWO"] ?? 0) + 2;
          workloadScore += 2;
        }
        if (answer === "D") {
          // Financial trends
          interestCandidates.push("Data", "AI / ML", "Projectmanagement");
          academyVotes["AMF"] = (academyVotes["AMF"] ?? 0) + 3;
          academyVotes["AMBF"] = (academyVotes["AMBF"] ?? 0) + 2;
          workloadScore += 1;
        }
        if (answer === "E") {
          // Biotech solution
          interestCandidates.push("Data", "Sustainability", "AI / ML");
          academyVotes["ALST"] = (academyVotes["ALST"] ?? 0) + 3;
          academyVotes["ATIx"] = (academyVotes["ATIx"] ?? 0) + 1;
          workloadScore += 1;
        }
        break;

      case 2: // environment - What environment do you thrive in?
        if (answer === "A") {
          // Supportive and people-focused
          interestCandidates.push("Projectmanagement", "UX / Product", "Design");
          academyVotes["AWG"] = (academyVotes["AWG"] ?? 0) + 2;
          academyVotes["AWEG"] = (academyVotes["AWEG"] ?? 0) + 2;
          workloadScore -= 1;
        }
        if (answer === "B") {
          // Fast-paced and entrepreneurial
          interestCandidates.push("Web Development", "Projectmanagement", "AI / ML");
          academyVotes["ABE"] = (academyVotes["ABE"] ?? 0) + 3;
          academyVotes["AWO"] = (academyVotes["AWO"] ?? 0) + 2;
          workloadScore += 2;
        }
        if (answer === "C") {
          // Analytical and structured
          interestCandidates.push("Data", "AI / ML", "Security");
          academyVotes["AMF"] = (academyVotes["AMF"] ?? 0) + 3;
          academyVotes["AMBF"] = (academyVotes["AMBF"] ?? 0) + 2;
          workloadScore += 1;
        }
        if (answer === "D") {
          // Creative and experimental
          interestCandidates.push("Design", "Sustainability", "UX / Product");
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 3;
          academyVotes["AACI"] = (academyVotes["AACI"] ?? 0) + 2;
          academyVotes["ADGO"] = (academyVotes["ADGO"] ?? 0) + 1;
        }
        if (answer === "E") {
          // Technical and research-driven
          interestCandidates.push("Embedded", "AI / ML", "Robotics", "Data");
          academyVotes["ATIx"] = (academyVotes["ATIx"] ?? 0) + 3;
          academyVotes["ALST"] = (academyVotes["ALST"] ?? 0) + 2;
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 1;
          workloadScore += 1;
        }
        break;

      case 3: // subjects - Which subjects do you enjoy?
        if (answer === "A") {
          // Psychology or social work
          interestCandidates.push("Sustainability", "UX / Product");
          academyVotes["AWG"] = (academyVotes["AWG"] ?? 0) + 3;
          academyVotes["AWEG"] = (academyVotes["AWEG"] ?? 0) + 2;
          workloadScore -= 1;
        }
        if (answer === "B") {
          // Economics or business
          interestCandidates.push("Projectmanagement", "Data", "Web Development");
          academyVotes["AMF"] = (academyVotes["AMF"] ?? 0) + 3;
          academyVotes["ABE"] = (academyVotes["ABE"] ?? 0) + 2;
          academyVotes["AMBF"] = (academyVotes["AMBF"] ?? 0) + 1;
          workloadScore += 1;
        }
        if (answer === "C") {
          // Biology or chemistry
          interestCandidates.push("Data", "Sustainability", "AI / ML");
          academyVotes["ALST"] = (academyVotes["ALST"] ?? 0) + 3;
          academyVotes["AWG"] = (academyVotes["AWG"] ?? 0) + 1;
        }
        if (answer === "D") {
          // Architecture or sustainability
          interestCandidates.push("Design", "Sustainability", "Projectmanagement");
          academyVotes["ADGO"] = (academyVotes["ADGO"] ?? 0) + 3;
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 2;
        }
        if (answer === "E") {
          // Art, media, or design
          interestCandidates.push("Design", "UX / Product", "Web Development");
          academyVotes["AACI"] = (academyVotes["AACI"] ?? 0) + 3;
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 1;
        }
        break;

      case 4: // problem solving - How do you solve problems?
        if (answer === "A") {
          // Empathizing
          interestCandidates.push("UX / Product", "Projectmanagement", "Design");
          academyVotes["AACI"] = (academyVotes["AACI"] ?? 0) + 2;
          academyVotes["AWG"] = (academyVotes["AWG"] ?? 0) + 2;
          workloadScore -= 1;
        }
        if (answer === "B") {
          // Designing and prototyping
          interestCandidates.push("Design", "Embedded", "UX / Product", "Robotics");
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 3;
          academyVotes["AACI"] = (academyVotes["AACI"] ?? 0) + 2;
          academyVotes["ATIx"] = (academyVotes["ATIx"] ?? 0) + 1;
          workloadScore += 1;
        }
        if (answer === "C") {
          // Data-driven decisions
          interestCandidates.push("Data", "AI / ML", "Web Development");
          academyVotes["AMF"] = (academyVotes["AMF"] ?? 0) + 3;
          academyVotes["AMBF"] = (academyVotes["AMBF"] ?? 0) + 2;
          academyVotes["ABE"] = (academyVotes["ABE"] ?? 0) + 1;
          workloadScore += 2;
        }
        if (answer === "D") {
          // Thinking creatively
          interestCandidates.push("Design", "UX / Product", "Sustainability");
          academyVotes["AACI"] = (academyVotes["AACI"] ?? 0) + 3;
          academyVotes["ATD"] = (academyVotes["ATD"] ?? 0) + 1;
        }
        break;
    }
  };

  // Process all answers
  answers.forEach((ans, idx) => {
    if (ans) mapAnswer(idx, ans);
  });

  // Determine top academy with proper name resolution
  let topAcademy: string | undefined = undefined;
  if (Object.keys(academyVotes).length > 0) {
    const sorted = Object.entries(academyVotes).sort((a, b) => b[1] - a[1]);
    const topAbbrev = sorted[0][0];
    topAcademy = ACADEMY_NAMES[topAbbrev] || topAbbrev;
  }

  // Deduplicate and normalize interests
  const interestFrequency: Record<string, number> = {};
  interestCandidates.forEach((interest) => {
    const normalized = interests.find(
      (i) =>
        i.toLowerCase() === interest.toLowerCase() ||
        i.toLowerCase().includes(interest.toLowerCase()) ||
        interest.toLowerCase().includes(i.toLowerCase()),
    );
    if (normalized) {
      interestFrequency[normalized] = (interestFrequency[normalized] ?? 0) + 1;
    }
  });

  // Sort interests by frequency and select top ones
  const sortedInterests = Object.entries(interestFrequency)
    .sort((a, b) => b[1] - a[1])
    .map(([interest]) => interest)
    .slice(0, 5); // Limit to top 5 interests

  // Compute workload preference with improved thresholds
  let workloadPref: "light" | "medium" | "heavy" = "medium";
  if (workloadScore <= -1) workloadPref = "light";
  else if (workloadScore >= 3) workloadPref = "heavy";

  return {
    academy: topAcademy,
    interests: sortedInterests,
    workloadPref,
    language: undefined,
  };
}

import { useState, useCallback } from "react";
import { Card } from "@/components/ui/card";
import { Toaster } from "@/components/ui/sonner";
import type { Elective } from "@/types/Elective";

import { scoreElective, diversifyResults } from "./scoring";
import { deriveFromPersonality } from "./personality";
import { useElectives } from "../../hooks/useElectives";
import { useFavorites } from "../../hooks/useFavorites";

import QuestionnaireHeader from "../../components/recommendations/QuestionnaireHeader";
import ProgressBar from "../../components/recommendations/ProgressBar";
import AcademyStep from "../../components/recommendations/AcademyStep";
import InterestsStep from "../../components/recommendations/InterestsStep";
import PersonalityStep from "../../components/recommendations/PersonalityStep";
import LanguageStep from "../../components/recommendations/LanguageStep";
import WorkloadStep from "../../components/recommendations/WorkloadStep";
import ResultsView from "../../components/recommendations/ResultsView";

const TOTAL_STEPS = 6;

export default function QuestionnaireRecommendations() {
  const [step, setStep] = useState<number>(1);
  const { electives, loading, error: fetchError } = useElectives();
  const { addFavorite } = useFavorites();

  const getAcademies = (): string[] => {
    const academySet = new Set<string>();
    electives?.forEach((e) => {
      if (e.provider) {
        academySet.add(e.provider);
      }
    });
    return Array.from(academySet).sort();
  };

  const getLanguages = (): string[] => {
    const languageSet = new Set<string>();
    electives?.forEach((e) => {
      if (e.language) {
        languageSet.add(e.language);
      }
    });
    return Array.from(languageSet).sort();
  };

  // Form state
  const [academy, setAcademy] = useState<string | undefined>(undefined);
  const [selectedInterests, setSelectedInterests] = useState<string[]>([]);
  const [language, setLanguage] = useState<string | undefined>(undefined);
  const [workloadPref, setWorkloadPref] = useState<"light" | "medium" | "heavy">("light");
  const [limit, setLimit] = useState<number>(6);
  const [personalityAnswers, setPersonalityAnswers] = useState<Array<string | null>>(
    Array(PERSONALITY_QUESTIONS.length).fill(null),
  );

  const [results, setResults] = useState<
    Array<{ elective: Elective; score: number; reasons: string[] }>
  >([]);

  const toggleInterest = (i: string) =>
    setSelectedInterests((prev) => (prev.includes(i) ? prev.filter((p) => p !== i) : [...prev, i]));

  const setPersonalityAnswer = (index: number, value: string) =>
    setPersonalityAnswers((prev) => prev.map((v, i) => (i === index ? value : v)));

  const computeRecommendations = useCallback(() => {
    if (!electives) return;

    const derived = deriveFromPersonality(personalityAnswers, INTERESTS);

    const finalAcademy = academy ?? derived.academy;
    const combinedInterests = Array.from(new Set([...selectedInterests, ...derived.interests]));
    const finalWorkload = workloadPref ?? derived.workloadPref;
    const finalLanguage = language ?? derived.language;

    const scored = electives.map((e) => {
      const { score, reasons } = scoreElective(e, {
        academy: finalAcademy,
        interests: combinedInterests,
        language: finalLanguage,
        workloadPref: finalWorkload,
      });
      return { elective: e, score, reasons };
    });

    // Sort by score, then by name
    scored.sort((a, b) => b.score - a.score || a.elective.name.localeCompare(b.elective.name));

    // Apply diversity boosting to prevent all results from same provider
    const diversified = diversifyResults(scored, 3);

    setResults(diversified.slice(0, limit));
    setStep(6);
  }, [electives, academy, selectedInterests, language, workloadPref, personalityAnswers, limit]);

  const resetQuestionnaire = () => {
    setAcademy(undefined);
    setSelectedInterests([]);
    setLanguage(undefined);
    setWorkloadPref("light");
    setLimit(6);
    setPersonalityAnswers(Array(PERSONALITY_QUESTIONS.length).fill(null));
    setResults([]);
    setStep(1);
  };

  const progressPct = Math.round(((Math.min(step, TOTAL_STEPS - 1) - 1) / (TOTAL_STEPS - 2)) * 100);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh] text-muted-foreground animate-pulse">
        Modules laden...
      </div>
    );
  }

  if (fetchError) {
    return (
      <div className="p-6">
        <p className="text-destructive mb-4">{fetchError}</p>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto px-4 py-10 space-y-8">
      <Toaster />

      <QuestionnaireHeader currentStep={step} totalSteps={TOTAL_STEPS} />

      <ProgressBar percentage={progressPct} />

      <Card className="rounded-2xl border border-border/40 p-4">
        {step === 1 && (
          <AcademyStep
            academy={academy}
            onAcademyChange={setAcademy}
            allAcademies={getAcademies()}
            onNext={() => setStep(2)}
          />
        )}

        {step === 2 && (
          <InterestsStep
            selectedInterests={selectedInterests}
            onToggleInterest={toggleInterest}
            allInterests={INTERESTS}
            onBack={() => setStep(1)}
            onNext={() => setStep(3)}
          />
        )}

        {step === 3 && (
          <PersonalityStep
            personalityAnswers={personalityAnswers}
            onAnswerChange={setPersonalityAnswer}
            allPersonalityQuestions={PERSONALITY_QUESTIONS}
            onBack={() => setStep(2)}
            onNext={() => setStep(4)}
          />
        )}

        {step === 4 && (
          <LanguageStep
            language={language}
            onLanguageChange={setLanguage}
            allLanguages={getLanguages()}
            onBack={() => setStep(3)}
            onNext={() => setStep(5)}
          />
        )}

        {step === 5 && (
          <WorkloadStep
            workloadPref={workloadPref}
            limit={limit}
            onWorkloadChange={setWorkloadPref}
            onLimitChange={setLimit}
            onBack={() => setStep(4)}
            onGenerateRecommendations={computeRecommendations}
          />
        )}
      </Card>

      {step === 6 && (
        <ResultsView results={results} onReset={resetQuestionnaire} onAddFavorite={addFavorite} />
      )}
    </div>
  );
}

// Constants for the recommendations questionnaire

export const INTERESTS: string[] = [
  "AI / ML",
  "Web Development",
  "Data",
  "Embedded",
  "Security",
  "Robotics",
  "Design",
  "Projectmanagement",
  "Sustainability",
  "UX / Product",
] as const;

export const PERSONALITY_QUESTIONS: Array<{
  id: number;
  q: string;
  options: Array<{ key: string; text: string }>;
}> = [
  {
    id: 1,
    q: "What kind of impact do you want to make in the world?",
    options: [
      { key: "A", text: "Improve people's health and well-being" },
      { key: "B", text: "Create innovative products or designs" },
      { key: "C", text: "Help businesses grow and succeed" },
      { key: "D", text: "Shape sustainable environments and communities" },
      { key: "E", text: "Inspire others through education and creativity" },
    ],
  },
  {
    id: 2,
    q: "Which type of project excites you most?",
    options: [
      { key: "A", text: "Designing a wellness program for youth" },
      { key: "B", text: "Building a smart home prototype" },
      { key: "C", text: "Launching a startup or social enterprise" },
      { key: "D", text: "Analyzing financial trends and advising clients" },
      { key: "E", text: "Developing a biotech solution for clean water" },
    ],
  },
  {
    id: 3,
    q: "What kind of environment do you thrive in?",
    options: [
      { key: "A", text: "Supportive and people-focused" },
      { key: "B", text: "Fast-paced and entrepreneurial" },
      { key: "C", text: "Analytical and structured" },
      { key: "D", text: "Creative and experimental" },
      { key: "E", text: "Technical and research-driven" },
    ],
  },
  {
    id: 4,
    q: "Which of these subjects do you enjoy most?",
    options: [
      { key: "A", text: "Psychology or social work" },
      { key: "B", text: "Economics or business strategy" },
      { key: "C", text: "Biology or chemistry" },
      { key: "D", text: "Architecture or sustainability" },
      { key: "E", text: "Art, media, or design" },
    ],
  },
  {
    id: 5,
    q: "How do you prefer to solve problems?",
    options: [
      { key: "A", text: "By empathizing and understanding people's needs" },
      { key: "B", text: "By designing and prototyping new solutions" },
      { key: "C", text: "By crunching numbers and making data-driven decisions" },
      { key: "D", text: "By thinking creatively and challenging norms" },
    ],
  },
] as const;

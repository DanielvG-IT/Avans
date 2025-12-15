// Step 3: Personality questions

import { Button } from "@/components/ui/button";
import { toast } from "sonner";

interface PersonalityStepProps {
  personalityAnswers: Array<string | null>;
  onAnswerChange: (index: number, value: string) => void;
  allPersonalityQuestions: Array<{
    id: number;
    q: string;
    options: Array<{ key: string; text: string }>;
  }>;
  onBack: () => void;
  onNext: () => void;
}

export default function PersonalityStep({
  personalityAnswers,
  onAnswerChange,
  allPersonalityQuestions,
  onBack,
  onNext,
}: PersonalityStepProps) {
  const handleNext = () => {
    const allAnswered = personalityAnswers.every((a) => a !== null);
    if (!allAnswered) {
      toast.error("Beantwoord alle vragen voordat je verdergaat.", {
        style: { background: "#ff4d4f", color: "white" },
      });
      return;
    }
    onNext();
  };

  return (
    <>
      <h2 className="text-lg font-semibold">Korte vragen — wie ben jij?</h2>
      <p className="text-sm text-muted-foreground">
        Beantwoord deze 5 vragen (A–E). We gebruiken je antwoorden om betere aanbevelingen te geven.
      </p>

      <div className="space-y-4 mt-4">
        {allPersonalityQuestions.map((q, qi) => (
          <div key={q.id} className="p-3 border rounded-md">
            <div className="font-medium">{q.q}</div>
            <div className="flex flex-col gap-2 mt-2">
              {q.options.map((opt) => {
                const selected = personalityAnswers[qi] === opt.key;
                return (
                  <button
                    key={opt.key}
                    onClick={() => onAnswerChange(qi, opt.key)}
                    className={`text-left px-3 py-2 rounded-md transition border ${
                      selected
                        ? "bg-primary text-white border-primary"
                        : "bg-muted/10 border-border/30"
                    }`}
                  >
                    <span className="font-semibold mr-2">{opt.key}.</span>
                    <span className="text-sm">{opt.text}</span>
                  </button>
                );
              })}
            </div>
          </div>
        ))}
      </div>

      <div className="flex justify-between mt-4">
        <Button variant="outline" onClick={onBack}>
          ← Terug
        </Button>
        <Button onClick={handleNext}>Volgende →</Button>
      </div>
    </>
  );
}

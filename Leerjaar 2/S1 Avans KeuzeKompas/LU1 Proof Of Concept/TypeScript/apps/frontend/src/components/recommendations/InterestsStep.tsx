// Step 2: Interests selection

import { Button } from "@/components/ui/button";

interface InterestsStepProps {
  selectedInterests: string[];
  onToggleInterest: (interest: string) => void;
  allInterests: string[];
  onBack: () => void;
  onNext: () => void;
}

export default function InterestsStep({
  selectedInterests,
  onToggleInterest,
  allInterests,
  onBack,
  onNext,
}: InterestsStepProps) {
  return (
    <>
      <h2 className="text-lg font-semibold">Wat zijn jouw interesses?</h2>
      <p className="text-sm text-muted-foreground">Kies meerdere interesses.</p>

      <div className="flex flex-wrap gap-2 mt-3">
        {allInterests.map((i) => {
          const active = selectedInterests.includes(i);
          return (
            <button
              key={i}
              onClick={() => onToggleInterest(i)}
              className={`px-3 py-1 rounded-full text-sm border transition ${
                active
                  ? "bg-primary text-white border-primary"
                  : "bg-muted/10 text-foreground border-border/30"
              }`}
            >
              {i}
            </button>
          );
        })}
      </div>

      <div className="flex justify-between mt-4">
        <Button variant="outline" onClick={onBack}>
          ← Terug
        </Button>
        <Button onClick={onNext}>Volgende →</Button>
      </div>
    </>
  );
}

// Step 1: Academy selection

import { Button } from "@/components/ui/button";

interface AcademyStepProps {
  academy?: string;
  onAcademyChange: (academy: string) => void;
  allAcademies: string[];
  onNext: () => void;
}

export default function AcademyStep({
  academy,
  onAcademyChange,
  allAcademies,
  onNext,
}: AcademyStepProps) {
  return (
    <>
      <h2 className="text-lg font-semibold">
        Bij welke academie zit jouw opleiding op dit moment?
      </h2>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 mt-3">
        {allAcademies.map((a) => (
          <button
            key={a}
            onClick={() => onAcademyChange(a)}
            className={`text-left p-3 rounded-xl border transition ${
              a === academy
                ? "bg-primary text-white border-primary"
                : "bg-muted/10 border-border/40"
            }`}
          >
            <div className="font-medium text-sm">{a}</div>
          </button>
        ))}
      </div>

      <div className="flex justify-end gap-2 mt-4">
        <Button variant="ghost" onClick={onNext}>
          Volgende →
        </Button>
      </div>
    </>
  );
}

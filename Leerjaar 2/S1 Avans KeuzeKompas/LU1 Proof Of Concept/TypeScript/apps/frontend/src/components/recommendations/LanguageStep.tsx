// Step 4: Language preference

import { Button } from "@/components/ui/button";

interface LanguageStepProps {
  language?: string;
  onLanguageChange: (language?: string) => void;
  allLanguages: string[];
  onBack: () => void;
  onNext: () => void;
}

export default function LanguageStep({
  language,
  onLanguageChange,
  allLanguages,
  onBack,
  onNext,
}: LanguageStepProps) {
  return (
    <>
      <h2 className="text-lg font-semibold">Taalvoorkeur</h2>
      <p className="text-sm text-muted-foreground">
        Heb je een voorkeur voor de taal van de aanbevolen modules?
      </p>

      <div className="flex gap-2 mt-3">
        <button
          className={`px-4 py-2 rounded-md ${language === undefined ? "bg-primary text-white" : "bg-muted/10"}`}
          onClick={() => onLanguageChange(undefined)}
        >
          Geen voorkeur
        </button>
        {allLanguages.map((lang) => (
          <button
            className={`px-4 py-2 rounded-md ${language === lang ? "bg-primary text-white" : "bg-muted/10"}`}
            onClick={() => onLanguageChange(lang)}
          >
            {lang}
          </button>
        ))}
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

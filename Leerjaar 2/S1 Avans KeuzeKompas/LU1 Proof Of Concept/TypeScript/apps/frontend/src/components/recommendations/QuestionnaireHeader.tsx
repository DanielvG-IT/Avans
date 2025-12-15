// Header component for the questionnaire

interface QuestionnaireHeaderProps {
  currentStep: number;
  totalSteps: number;
}

export default function QuestionnaireHeader({ currentStep, totalSteps }: QuestionnaireHeaderProps) {
  return (
    <header className="flex items-center justify-between">
      <div>
        <h1 className="text-3xl font-bold">Vragenlijst — Aanbevelingen</h1>
        <p className="text-sm text-muted-foreground mt-1">
          Beantwoord korte vragen zodat we geschikte keuzemodules voor je kunnen voorstellen.
        </p>
      </div>

      <div className="text-sm text-muted-foreground">
        Stap {Math.min(currentStep, totalSteps)} / {totalSteps}
      </div>
    </header>
  );
}

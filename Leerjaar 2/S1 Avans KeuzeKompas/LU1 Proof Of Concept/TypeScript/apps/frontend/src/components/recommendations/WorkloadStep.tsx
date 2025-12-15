// Step 5: Workload & limit preferences

import { Button } from "@/components/ui/button";

interface WorkloadStepProps {
  workloadPref: "light" | "medium" | "heavy";
  limit: number;
  onWorkloadChange: (workload: "light" | "medium" | "heavy") => void;
  onLimitChange: (limit: number) => void;
  onBack: () => void;
  onGenerateRecommendations: () => void;
}

export default function WorkloadStep({
  workloadPref,
  limit,
  onWorkloadChange,
  onLimitChange,
  onBack,
  onGenerateRecommendations,
}: WorkloadStepProps) {
  return (
    <>
      <h2 className="text-lg font-semibold">Workload & aantal resultaten</h2>
      <p className="text-sm text-muted-foreground">
        Kies voorkeur voor workload en hoeveel aanbevelingen je wilt zien.
      </p>

      <div className="flex gap-2 mt-3">
        <button
          onClick={() => onWorkloadChange("light")}
          className={`px-4 py-2 rounded-md ${workloadPref === "light" ? "bg-primary text-white" : "bg-muted/10"}`}
        >
          Licht
        </button>
        <button
          onClick={() => onWorkloadChange("medium")}
          className={`px-4 py-2 rounded-md ${workloadPref === "medium" ? "bg-primary text-white" : "bg-muted/10"}`}
        >
          Gemiddeld
        </button>
        <button
          onClick={() => onWorkloadChange("heavy")}
          className={`px-4 py-2 rounded-md ${workloadPref === "heavy" ? "bg-primary text-white" : "bg-muted/10"}`}
        >
          Zwaar
        </button>
      </div>

      <div className="mt-3 w-48">
        <label className="text-sm block mb-1">Aantal resultaten</label>
        <input
          type="range"
          min={1}
          max={12}
          value={limit}
          onChange={(e) => onLimitChange(Number(e.target.value))}
          className="w-full"
        />
        <div className="text-xs text-muted-foreground mt-1">{limit} resultaten</div>
      </div>

      <div className="flex justify-between mt-4">
        <Button variant="outline" onClick={onBack}>
          ← Terug
        </Button>
        <Button onClick={onGenerateRecommendations}>Genereer aanbevelingen →</Button>
      </div>
    </>
  );
}

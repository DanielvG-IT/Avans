// Results view component

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { toast } from "sonner";
import type { Elective } from "@/types/Elective";

interface ResultItem {
  elective: Elective;
  score: number;
  reasons: string[];
}

interface ResultsViewProps {
  results: ResultItem[];
  onReset: () => void;
  onAddFavorite: (electiveId: string) => void;
}

export default function ResultsView({ results, onReset, onAddFavorite }: ResultsViewProps) {
  return (
    <section>
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold">Top {results.length} aanbevelingen</h2>
        <div className="flex gap-2">
          <Button variant="outline" onClick={onReset}>
            Nieuwe vragenlijst
          </Button>
          <Button
            onClick={() =>
              toast("Aanbevelingen vernieuwd", {
                style: { background: "#60a5fa", color: "white" },
              })
            }
          >
            🔁 Vernieuw
          </Button>
        </div>
      </div>

      <div className="mt-4 grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {results.map(({ elective, score, reasons }) => (
          <Card
            key={elective.id}
            className="rounded-2xl border border-border/40 hover:shadow-md transition"
          >
            <CardHeader className="p-4">
              <div className="flex items-start justify-between gap-2">
                <div>
                  <CardTitle className="text-base">{elective.name}</CardTitle>
                  <CardDescription className="text-xs text-muted-foreground">
                    {elective.code} • {elective.provider}
                  </CardDescription>
                </div>

                <div className="text-right">
                  <div className="text-sm font-semibold">{score}%</div>
                  <div className="text-xs text-muted-foreground">match</div>
                </div>
              </div>
            </CardHeader>

            <CardContent className="p-4 space-y-2">
              <p className="text-sm text-muted-foreground line-clamp-3">
                {elective.description ?? "Geen omschrijving beschikbaar."}
              </p>

              <div className="flex gap-2 flex-wrap">
                {(elective.tags ?? []).slice(0, 5).map((t) => (
                  <Badge key={t} variant="secondary" className="text-xs">
                    {t}
                  </Badge>
                ))}
                {elective.credits != null && (
                  <Badge variant="outline" className="text-xs">
                    {elective.credits} EC
                  </Badge>
                )}
              </div>

              <div className="text-xs text-muted-foreground">
                <strong>Waarom:</strong> {reasons.length ? reasons.join(", ") : "Algemene match"}
              </div>

              <div className="flex gap-2 pt-2">
                <Button
                  onClick={() => (window.location.href = `/electives/${elective.id}`)}
                  variant="outline"
                >
                  Details →
                </Button>
                <Button
                  onClick={() => {
                    if (elective.id) onAddFavorite(elective.id);
                  }}
                >
                  ☆ Favoriet
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </section>
  );
}

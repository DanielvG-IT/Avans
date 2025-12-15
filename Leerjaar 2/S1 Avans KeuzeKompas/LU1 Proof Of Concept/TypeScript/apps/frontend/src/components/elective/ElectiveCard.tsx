import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import ProviderBadge from "@/components/elective/ProviderBadge";
import { Badge } from "@/components/ui/badge";
import { useNavigate } from "react-router-dom";
import type { Elective } from "@/types/Elective";
import { memo, useMemo } from "react";

interface Props {
  elective: Elective;
}

const ElectiveCard = ({ elective }: Props) => {
  const {
    id: electiveId,
    name,
    code: electiveCode,
    language,
    provider,
    description,
    period,
    duration,
    credits,
    level,
    location,
  } = elective;

  const meta = useMemo(
    () =>
      [
        { key: "period", label: period },
        { key: "duration", label: duration },
        { key: "credits", label: credits != null ? `${credits} EC` : undefined },
        { key: "level", label: level },
        { key: "location", label: location },
      ].filter((m) => m.label) as { key: string; label: string }[],
    [period, duration, credits, level, location],
  );

  const navigate = useNavigate();

  const safeDescription =
    description?.trim() && description.length > 20 ? description : "No description available.";

  return (
    <Card
      className="
        group relative flex flex-col h-full border border-border/40 
        rounded-2xl shadow-sm bg-gradient-to-b from-background to-muted/10 
        hover:shadow-lg hover:border-primary/40 
        hover:bg-gradient-to-b hover:from-muted/10 hover:to-background 
        transition-all duration-200 cursor-pointer
        focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2
      "
      role="article"
      tabIndex={0}
      onClick={() => {
        navigate(`/electives/${encodeURIComponent(electiveId as string)}`, {
          replace: false,
          state: { from: window.location.pathname },
        });
      }}
      onKeyDown={(e) => {
        if (e.key === "Enter" || e.key === " " || e.key === "Spacebar") {
          e.preventDefault();
          navigate(`/electives/${encodeURIComponent(electiveId as string)}`, {
            replace: false,
            state: { from: window.location.pathname },
          });
        }
      }}
    >
      <CardHeader className="pb-2">
        <div className="flex items-start justify-between gap-3">
          <div className="min-w-0">
            <CardTitle
              className="
                text-base sm:text-lg font-semibold leading-tight line-clamp-2 
                text-foreground group-hover:text-primary transition-colors
              "
            >
              {name}
            </CardTitle>
            <CardDescription className="text-xs sm:text-sm mt-1 truncate">
              {electiveCode} • {language}
            </CardDescription>
          </div>

          <div className="shrink-0">
            <ProviderBadge provider={provider} codeOnly={true} />
          </div>
        </div>
      </CardHeader>

      <CardContent className="flex flex-col flex-1 justify-between space-y-3">
        <p
          title={safeDescription}
          className="text-sm text-muted-foreground line-clamp-3 leading-relaxed"
        >
          {safeDescription}
        </p>

        <div
          className="
            flex flex-wrap gap-2 mt-auto pt-2
            border-t border-border/40
          "
          aria-hidden={meta.length === 0}
        >
          {meta.map((m) => (
            <Badge
              key={m.key}
              variant="secondary"
              className="
                text-xs font-medium bg-muted/60 hover:bg-muted 
                transition-colors border border-border/30
              "
              title={m.label}
            >
              {m.label}
            </Badge>
          ))}
        </div>
      </CardContent>
    </Card>
  );
};

const MemoElectiveCard = memo(ElectiveCard);
MemoElectiveCard.displayName = "ElectiveCard";

export default MemoElectiveCard;

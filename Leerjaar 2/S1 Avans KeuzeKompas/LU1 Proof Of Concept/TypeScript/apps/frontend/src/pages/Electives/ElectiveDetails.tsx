import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import ProviderBadge from "@/components/elective/ProviderBadge";
import { RoleProtected } from "@/components/auth/RoleProtected";
import { useMemo, useCallback, useState } from "react";
import { useFavorites } from "@/hooks/useFavorites";
import { useElective } from "@/hooks/useElective";
import { Toaster } from "@/components/ui/sonner";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { ElectiveEditDialog } from "@/components/elective/ElectiveFormDialog";

const ElectiveDetailPage = () => {
  const { electiveId } = useParams<{ electiveId: string }>();
  const navigate = useNavigate();
  const location = useLocation();

  // Use the new custom hook for fetching elective data
  const { elective, loading, error, isFavorited, setIsFavorited, refetch } =
    useElective(electiveId);
  const { toggleFavorite: toggleFavoriteApi, loading: favoriteLoading } = useFavorites();

  // Edit dialog state
  const [editDialogOpen, setEditDialogOpen] = useState(false);

  // Favorite toggle with optimistic UI
  const toggleFavorite = useCallback(async () => {
    if (!elective?.id || favoriteLoading) return;

    const prevFavorited = isFavorited;
    setIsFavorited(!prevFavorited);

    try {
      await toggleFavoriteApi(elective.id, prevFavorited);
    } catch (err) {
      // Rollback on error
      setIsFavorited(prevFavorited);
    }
  }, [elective?.id, isFavorited, favoriteLoading, toggleFavoriteApi, setIsFavorited]);

  // Meta info
  const meta = useMemo(() => {
    if (!elective) return [];
    return [
      { key: "period", label: elective.period },
      { key: "duration", label: elective.duration },
      { key: "credits", label: elective.credits != null ? `${elective.credits} EC` : undefined },
      { key: "level", label: elective.level },
      { key: "location", label: elective.location },
      { key: "language", label: elective.language },
    ].filter((m) => m.label) as { key: string; label: string }[];
  }, [elective]);

  if (loading)
    return (
      <div className="flex items-center justify-center min-h-[60vh] bg-background">
        <div className="flex flex-col items-center gap-4 p-8 bg-card border border-border rounded-lg">
          <div className="w-8 h-8 border-2 border-t-2 border-primary rounded-full animate-spin" />
          <p className="text-muted-foreground animate-pulse">Loading elective details...</p>
        </div>
      </div>
    );

  if (error || !elective)
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh] text-center p-6 bg-background">
        <div className="bg-card border border-border rounded-lg p-8 max-w-md">
          <p className="text-muted-foreground mb-4">{error ?? "Elective not found."}</p>
          <Button onClick={() => navigate(-1)} variant="outline">
            Go Back
          </Button>
        </div>
      </div>
    );

  return (
    <div className="max-w-6xl mx-auto px-3 sm:px-4 md:px-6 lg:px-8 py-6 sm:py-8 md:py-10 space-y-6 sm:space-y-8 bg-background min-h-screen">
      {/* Toast container */}
      <Toaster />

      {/* Hero Section */}
      <div className="relative rounded-2xl sm:rounded-3xl overflow-hidden shadow-lg group border border-border">
        <img
          src="/keuzemodule_fallback_16-9.webp"
          alt={`${elective.name} banner`}
          className="w-full h-48 sm:h-64 md:h-80 lg:h-96 object-cover transform group-hover:scale-[1.02] transition-transform duration-500"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-background/95 via-background/60 to-transparent backdrop-blur-[2px]" />
        <div className="absolute bottom-4 sm:bottom-6 md:bottom-8 left-4 sm:left-6 md:left-8 lg:left-10 drop-shadow-lg space-y-1 right-4 sm:right-6">
          <h1 className="text-xl sm:text-2xl md:text-3xl lg:text-4xl font-bold leading-tight text-foreground drop-shadow-md">
            {elective.name}
          </h1>
          <p className="text-xs sm:text-sm md:text-base text-foreground/90">
            {elective.code} • {elective.language}
          </p>
        </div>
      </div>

      {/* Provider + Meta */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 sm:gap-4 px-2">
        <ProviderBadge provider={elective.provider} />
        <div className="flex flex-wrap gap-2">
          {meta.map((m) => (
            <Badge
              key={m.key}
              variant="secondary"
              className="text-xs sm:text-sm font-medium bg-muted/70 border border-border/40 hover:bg-muted transition-all"
            >
              {m.label}
            </Badge>
          ))}
        </div>
      </div>

      {/* Description Card */}
      <Card className="border border-border/40 bg-gradient-to-b from-background to-muted/10 rounded-2xl sm:rounded-3xl shadow-md hover:shadow-lg transition-shadow">
        <CardHeader className="pb-2 px-4 sm:px-6">
          <CardTitle className="text-base sm:text-lg md:text-xl font-semibold tracking-tight text-foreground">
            About this Elective
          </CardTitle>
          <CardDescription className="text-xs sm:text-sm text-muted-foreground">
            Offered by <span className="font-medium text-foreground">{elective.provider}</span>
          </CardDescription>
        </CardHeader>

        <CardContent className="px-4 sm:px-6">
          <p className="text-muted-foreground leading-relaxed whitespace-pre-line text-sm sm:text-base">
            {elective.description?.trim() || "No detailed description available for this elective."}
          </p>
        </CardContent>
      </Card>

      {/* Teachers Card */}
      {elective.teachers && elective.teachers.length > 0 && (
        <Card className="border border-border/40 bg-gradient-to-b from-background to-muted/10 rounded-2xl sm:rounded-3xl shadow-md hover:shadow-lg transition-shadow">
          <CardHeader className="pb-2 px-4 sm:px-6">
            <CardTitle className="text-base sm:text-lg md:text-xl font-semibold tracking-tight text-foreground">
              👨‍🏫 Teaching Staff
            </CardTitle>
            <CardDescription className="text-xs sm:text-sm text-muted-foreground">
              {elective.teachers.length === 1 ? "Teacher" : "Teachers"} for this elective
            </CardDescription>
          </CardHeader>

          <CardContent className="px-4 sm:px-6">
            <div className="flex flex-wrap gap-3">
              {elective.teachers.map((teacher, index) => (
                <div
                  key={teacher.id || index}
                  className="flex items-center gap-3 px-3 sm:px-4 py-2 sm:py-3 bg-muted/50 border border-border/30 rounded-xl hover:bg-muted/70 transition-all"
                >
                  <div className="flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 bg-primary/10 border border-primary/20 rounded-full text-primary font-semibold text-xs sm:text-sm">
                    {teacher.firstName?.[0]}
                    {teacher.lastName?.[0]}
                  </div>
                  <div className="flex flex-col min-w-0">
                    <span className="text-xs sm:text-sm font-medium text-foreground truncate">
                      {teacher.firstName} {teacher.lastName}
                    </span>
                    <span className="text-xs text-muted-foreground truncate">{teacher.email}</span>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Footer Actions */}
      <div className="flex flex-col sm:flex-row sm:justify-between pt-4 gap-3">
        <Button
          variant="outline"
          onClick={() => navigate(location.state?.from ?? "/electives")}
          className="rounded-xl border-border/50 hover:border-primary/50 transition-all w-full sm:w-auto order-2 sm:order-1"
        >
          ← Back to Electives
        </Button>

        <div className="flex flex-col xs:flex-row gap-2 order-1 sm:order-2">
          {/* Only admins can edit */}
          <RoleProtected allowedRoles="admin">
            <Button
              variant="default"
              onClick={() => setEditDialogOpen(true)}
              className="rounded-xl transition-all w-full xs:w-auto"
            >
              ✏️ Edit Elective
            </Button>
          </RoleProtected>

          {/* Only students can add to favorites */}
          <RoleProtected allowedRoles="student">
            <Button
              variant={isFavorited ? "destructive" : "outline"}
              onClick={toggleFavorite}
              disabled={favoriteLoading}
              className={`rounded-xl border-border/50 hover:border-primary/50 transition-all w-full xs:w-auto ${
                favoriteLoading ? "animate-pulse" : ""
              }`}
            >
              {isFavorited ? "★ Favorited" : "☆ Add to Favorites"}
            </Button>
          </RoleProtected>
        </div>
      </div>

      {/* Edit Dialog */}
      {elective && (
        <ElectiveEditDialog
          open={editDialogOpen}
          onOpenChange={setEditDialogOpen}
          elective={elective}
          onSuccess={() => {
            refetch();
          }}
        />
      )}
    </div>
  );
};

export default ElectiveDetailPage;

/**
 * Custom hook for fetching a single elective with favorite status
 */

import { useEffect, useState, useCallback } from "react";
import { electivesApi, favoritesApi } from "@/services/api.service";
import type { Elective } from "@/types/Elective";
import { toast } from "sonner";

export function useElective(electiveId: string | undefined) {
  const [elective, setElective] = useState<Elective | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isFavorited, setIsFavorited] = useState(false);

  const fetchElective = useCallback(async () => {
    if (!electiveId) {
      setError("No elective ID provided");
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      // Fetch elective and favorite status in parallel
      const [electiveData, favoriteStatus] = await Promise.all([
        electivesApi.getById(electiveId),
        favoritesApi.checkFavorite(electiveId),
      ]);

      setElective(electiveData);
      setIsFavorited(favoriteStatus);
    } catch (err: any) {
      console.error(err);
      const errorMessage = err?.message ?? "Failed to load elective";
      setError(errorMessage);
      toast.error(errorMessage);
    } finally {
      setLoading(false);
    }
  }, [electiveId]);

  useEffect(() => {
    fetchElective();
  }, [fetchElective]);

  return { elective, loading, error, isFavorited, setIsFavorited, refetch: fetchElective };
}

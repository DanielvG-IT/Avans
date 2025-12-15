// Custom hook for managing favorites

import { toast } from "sonner";
import { favoritesApi } from "@/services/api.service";
import { useState, useCallback } from "react";

export function useFavorites() {
  const [loading, setLoading] = useState(false);

  const addFavorite = useCallback(async (electiveId: string) => {
    try {
      setLoading(true);
      await favoritesApi.add(electiveId);
      toast.success("Favoriet toegevoegd!", { style: { background: "#52c41a", color: "white" } });
    } catch (err: any) {
      console.error(err);
      toast.error(err?.message ?? "Kon favoriet niet toevoegen", {
        style: { background: "#ff4d4f", color: "white" },
      });
    } finally {
      setLoading(false);
    }
  }, []);

  const removeFavorite = useCallback(async (electiveId: string) => {
    try {
      setLoading(true);
      await favoritesApi.remove(electiveId);
      toast.success("Favoriet verwijderd!", { style: { background: "#52c41a", color: "white" } });
    } catch (err: any) {
      console.error(err);
      toast.error(err?.message ?? "Kon favoriet niet verwijderen", {
        style: { background: "#ff4d4f", color: "white" },
      });
    } finally {
      setLoading(false);
    }
  }, []);

  const toggleFavorite = useCallback(async (electiveId: string, isFavorited: boolean) => {
    try {
      setLoading(true);
      await favoritesApi.toggle(electiveId, isFavorited);
      toast.success(!isFavorited ? "Favoriet toegevoegd!" : "Favoriet verwijderd!", {
        style: { background: "#52c41a", color: "white" },
      });
    } catch (err: any) {
      console.error(err);
      toast.error(err?.message ?? "Kon favoriet status niet wijzigen", {
        style: { background: "#ff4d4f", color: "white" },
      });
      throw err; // Re-throw to allow caller to handle optimistic UI rollback
    } finally {
      setLoading(false);
    }
  }, []);

  return { addFavorite, removeFavorite, toggleFavorite, loading };
}

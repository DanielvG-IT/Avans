// Custom hook for loading electives

import { toast } from "sonner";
import { electivesApi } from "@/services/api.service";
import { useEffect, useState, useCallback } from "react";
import type { Elective } from "@/types/Elective";

export function useElectives() {
  const [electives, setElectives] = useState<Elective[] | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchElectives = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await electivesApi.getAll();
      setElectives(data);
    } catch (err: any) {
      console.error(err);
      const errorMessage = err?.message ?? "Kon modules niet laden";
      setError(errorMessage);
      toast.error(errorMessage, {
        style: { background: "#ff4d4f", color: "white" },
      });
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    let cancelled = false;

    const load = async () => {
      await fetchElectives();
    };

    if (!cancelled) {
      load();
    }

    return () => {
      cancelled = true;
    };
  }, [fetchElectives]);

  return { electives, loading, error, refetch: fetchElectives };
}

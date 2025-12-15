/**
 * Custom hook for fetching and managing the current authenticated user
 */

import { useEffect, useState, useCallback } from "react";
import { userApi } from "@/services/api.service";
import type { User } from "@/types/User";
import { toast } from "sonner";

export function useUser() {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchUser = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const userData = await userApi.getMe();
      setUser(userData);
    } catch (err: any) {
      console.error(err);
      const errorMessage = err?.message ?? "Failed to load user profile";
      setError(errorMessage);
      toast.error(errorMessage, {
        style: { background: "#ff4d4f", color: "white" },
      });
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchUser();
  }, [fetchUser]);

  return { user, loading, error, refetch: fetchUser };
}

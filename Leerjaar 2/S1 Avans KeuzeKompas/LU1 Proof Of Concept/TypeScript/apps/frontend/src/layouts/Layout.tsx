import React, { useEffect, useState } from "react";
import Header from "../components/Header";
import { Outlet } from "react-router-dom";
import type { User } from "../types/User";
import { userApi } from "@/services/api.service";
import { Toaster } from "@/components/ui/sonner";

export const Layout = (): React.ReactNode | null => {
  const [user, setUser] = useState<User | undefined>(undefined);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const userData = await userApi.getMe();
        setUser(userData);
      } catch (err) {
        console.error("Failed to fetch user:", err);
      }
    };
    fetchUser();
  }, []);

  const onLogout = () => {
    setUser(undefined);
    cookieStore.delete("ACCESSTOKEN");
    window.location.replace("/auth/login");
  };

  return (
    <>
      <Header user={user} onLogout={onLogout} />
      <main className="min-h-screen bg-background">
        <div className="w-full max-w-[100vw] overflow-x-hidden">
          <Outlet context={{ user }} />
        </div>
      </main>
      <Toaster richColors closeButton position="top-right" />
    </>
  );
};

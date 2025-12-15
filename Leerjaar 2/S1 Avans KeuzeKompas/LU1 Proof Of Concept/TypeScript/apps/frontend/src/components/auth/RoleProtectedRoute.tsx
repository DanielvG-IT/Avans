/**
 * Route protection component for role-based access control
 * Redirects users without proper roles to an access denied page
 * 
 * Usage:
 * <RoleProtectedRoute allowedRoles={["teacher", "admin"]}>
 *   <AdminDashboard />
 * </RoleProtectedRoute>
 */

import { useAuth } from "@/hooks/useAuth";
import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";

type Role = "student" | "teacher" | "admin";

interface RoleProtectedRouteProps {
  /** Single role or array of roles that are allowed to access the route */
  allowedRoles: Role | Role[];
  /** The page/component to render if user has access */
  children: ReactNode;
  /** Optional redirect path (defaults to home) */
  redirectTo?: string;
}

export function RoleProtectedRoute({
  allowedRoles,
  children,
  redirectTo = "/",
}: RoleProtectedRouteProps) {
  const { user, hasRole, loading } = useAuth();

  // Show loading state while checking authentication
  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh] bg-background">
        <div className="flex flex-col items-center gap-4 p-8 bg-card border border-border rounded-lg">
          <div className="w-8 h-8 border-2 border-t-2 border-primary rounded-full animate-spin" />
          <p className="text-muted-foreground animate-pulse">Checking permissions...</p>
        </div>
      </div>
    );
  }

  // If not authenticated, redirect to login
  if (!user) {
    return <Navigate to="/auth/login" replace />;
  }

  // Check if user has required role
  const hasAccess = hasRole(allowedRoles);

  // If user doesn't have access, show access denied or redirect
  if (!hasAccess) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh] text-center p-6 bg-background">
        <div className="bg-card border border-destructive/50 rounded-lg p-8 max-w-md space-y-4">
          <div className="text-6xl">🔒</div>
          <h2 className="text-2xl font-bold text-foreground">Access Denied</h2>
          <p className="text-muted-foreground">
            You don't have permission to access this page.
          </p>
          <p className="text-sm text-muted-foreground">
            Your role: <span className="font-semibold text-foreground">{user.role}</span>
          </p>
          <p className="text-sm text-muted-foreground">
            Required role(s):{" "}
            <span className="font-semibold text-foreground">
              {Array.isArray(allowedRoles) ? allowedRoles.join(", ") : allowedRoles}
            </span>
          </p>
          <div className="pt-4">
            <a
              href={redirectTo}
              className="inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground hover:bg-primary/90 h-10 px-4 py-2"
            >
              Go to Home
            </a>
          </div>
        </div>
      </div>
    );
  }

  // User has access, render the protected content
  return <>{children}</>;
}

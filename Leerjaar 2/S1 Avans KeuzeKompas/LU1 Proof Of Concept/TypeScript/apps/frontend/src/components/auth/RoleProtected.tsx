/**
 * Component for conditionally rendering content based on user roles
 * 
 * Usage examples:
 * 
 * // Show only to students
 * <RoleProtected allowedRoles="student">
 *   <StudentOnlyContent />
 * </RoleProtected>
 * 
 * // Show to teachers and admins
 * <RoleProtected allowedRoles={["teacher", "admin"]}>
 *   <AdminContent />
 * </RoleProtected>
 * 
 * // Show fallback when user doesn't have access
 * <RoleProtected allowedRoles="admin" fallback={<p>Access denied</p>}>
 *   <AdminPanel />
 * </RoleProtected>
 */

import { useAuth } from "@/hooks/useAuth";
import type { ReactNode } from "react";

type Role = "student" | "teacher" | "admin";

interface RoleProtectedProps {
  /** Single role or array of roles that are allowed to see the content */
  allowedRoles: Role | Role[];
  /** Content to render when user has the required role */
  children: ReactNode;
  /** Optional content to render when user doesn't have the required role */
  fallback?: ReactNode;
  /** If true, shows loading state while checking user role */
  showLoadingState?: boolean;
}

export function RoleProtected({
  allowedRoles,
  children,
  fallback = null,
  showLoadingState = false,
}: RoleProtectedProps) {
  const { hasRole, loading } = useAuth();

  // Show loading state if enabled
  if (loading && showLoadingState) {
    return (
      <div className="animate-pulse bg-muted/50 rounded-md h-10 w-full" />
    );
  }

  // Check if user has required role
  const hasAccess = hasRole(allowedRoles);

  // Render children if user has access, otherwise render fallback
  return hasAccess ? <>{children}</> : <>{fallback}</>;
}

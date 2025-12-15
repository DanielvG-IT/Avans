/**
 * Custom hook for authentication and role-based access control
 */

import { useUser } from "./useUser";

type Role = "student" | "teacher" | "admin";

export function useAuth() {
  const { user, loading, error, refetch } = useUser();

  /**
   * Check if the current user has one of the specified roles
   */
  const hasRole = (allowedRoles: Role | Role[]): boolean => {
    if (!user) return false;

    const roles = Array.isArray(allowedRoles) ? allowedRoles : [allowedRoles];
    return roles.includes(user.role);
  };

  /**
   * Check if the current user is a student
   */
  const isStudent = (): boolean => hasRole("student");

  /**
   * Check if the current user is a teacher
   */
  const isTeacher = (): boolean => hasRole("teacher");

  /**
   * Check if the current user is an admin
   */
  const isAdmin = (): boolean => hasRole("admin");

  /**
   * Check if the current user is a teacher or admin
   */
  const isTeacherOrAdmin = (): boolean => hasRole(["teacher", "admin"]);

  return {
    user,
    loading,
    error,
    refetch,
    hasRole,
    isStudent,
    isTeacher,
    isAdmin,
    isTeacherOrAdmin,
  };
}

/**
 * API Service Layer
 * Centralized API endpoint functions for reusability across the application
 */

import { fetchBackend } from "@/lib/fetch";
import type { Elective } from "@/types/Elective";
import type { User, StudentUser, TeacherUser } from "@/types/User";

// ============================================
// User API
// ============================================

export const userApi = {
  /**
   * Get the current user's profile
   */
  getMe: async (): Promise<User> => {
    const response = await fetchBackend("/api/users/me");
    if (!response.ok) {
      throw new Error(`Failed to fetch user profile: ${response.statusText}`);
    }
    return (await response.json()) as User;
  },

  /**
   * Get all users (admin only)
   */
  getAll: async (): Promise<User[]> => {
    const response = await fetchBackend("/api/users");
    if (!response.ok) {
      throw new Error(`Failed to fetch users: ${response.statusText}`);
    }
    return (await response.json()) as User[];
  },

  /**
   * Create a new user (admin only)
   */
  create: async (userData: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    role: "student" | "teacher" | "admin";
  }): Promise<User> => {
    const response = await fetchBackend("/api/users", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(userData),
    });
    if (!response.ok) {
      throw new Error(`Failed to create user: ${response.statusText}`);
    }
    return (await response.json()) as User;
  },

  /**
   * Update a user (admin only)
   */
  update: async (
    userId: string,
    userData: {
      firstName?: string;
      lastName?: string;
      email?: string;
    },
  ): Promise<User> => {
    const response = await fetchBackend(`/api/users/${userId}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(userData),
    });
    if (!response.ok) {
      throw new Error(`Failed to update user: ${response.statusText}`);
    }
    return (await response.json()) as User;
  },

  /**
   * Delete a user (admin only)
   */
  delete: async (userId: string): Promise<void> => {
    const response = await fetchBackend(`/api/users/${userId}`, {
      method: "DELETE",
    });
    if (!response.ok) {
      throw new Error(`Failed to delete user: ${response.statusText}`);
    }
  },

  /**
   * Helper to separate users by role
   */
  separateByRole(users: User[]): {
    students: StudentUser[];
    teachers: TeacherUser[];
  } {
    return {
      students: users.filter((user): user is StudentUser => user.role === "student"),
      teachers: users.filter((user): user is TeacherUser => user.role === "teacher"),
    };
  },
};

// ============================================
// Electives API
// ============================================

export const electivesApi = {
  /**
   * Get all electives
   */
  getAll: async (): Promise<Elective[]> => {
    const response = await fetchBackend("/api/electives");
    if (!response.ok) {
      throw new Error(`Failed to fetch electives: ${response.statusText}`);
    }
    return (await response.json()) as Elective[];
  },

  /**
   * Get a single elective by ID
   */
  getById: async (electiveId: string): Promise<Elective> => {
    const response = await fetchBackend(`/api/electives/${electiveId}`);
    if (!response.ok) {
      throw new Error(`Failed to fetch elective: ${response.statusText}`);
    }
    const data = (await response.json()) as Elective | null;
    if (!data) {
      throw new Error("Elective not found");
    }
    return data;
  },

  /**
   * Create a new elective (admin only)
   */
  create: async (
    electiveData: Omit<Elective, "id" | "createdAt" | "updatedAt">,
  ): Promise<Elective> => {
    const response = await fetchBackend("/api/electives", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(electiveData),
    });
    if (!response.ok) {
      throw new Error(`Failed to create elective: ${response.statusText}`);
    }
    return (await response.json()) as Elective;
  },

  /**
   * Update an existing elective (admin only)
   */
  update: async (
    electiveId: string,
    electiveData: Partial<Omit<Elective, "id" | "createdAt" | "updatedAt">>,
  ): Promise<Elective> => {
    const response = await fetchBackend(`/api/electives/${electiveId}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(electiveData),
    });
    if (!response.ok) {
      throw new Error(`Failed to update elective: ${response.statusText}`);
    }
    return (await response.json()) as Elective;
  },

  /**
   * Assign a teacher to an elective (admin only)
   */
  assignTeacher: async (electiveId: string, teacherId: string): Promise<void> => {
    const response = await fetchBackend(`/api/electives/${electiveId}/teachers/${teacherId}`, {
      method: "POST",
    });
    if (!response.ok) {
      throw new Error(`Failed to assign teacher: ${response.statusText}`);
    }
  },

  /**
   * Unassign a teacher from an elective (admin only)
   */
  unassignTeacher: async (electiveId: string, teacherId: string): Promise<void> => {
    const response = await fetchBackend(`/api/electives/${electiveId}/teachers/${teacherId}`, {
      method: "DELETE",
    });
    if (!response.ok) {
      throw new Error(`Failed to unassign teacher: ${response.statusText}`);
    }
  },
};

// ============================================
// Favorites API
// ============================================

export const favoritesApi = {
  /**
   * Get all favorites for the current user
   */
  getAll: async (): Promise<Elective[]> => {
    const response = await fetchBackend("/api/users/me/favorites");
    if (!response.ok) {
      throw new Error(`Failed to fetch favorites: ${response.statusText}`);
    }
    return (await response.json()) as Elective[];
  },

  /**
   * Check if an elective is favorited
   */
  checkFavorite: async (electiveId: string): Promise<boolean> => {
    const response = await fetchBackend(`/api/users/me/favorites/${electiveId}`);
    if (!response.ok) {
      return false;
    }
    const data = (await response.json()) as { isFavorite: boolean };
    return data.isFavorite;
  },

  /**
   * Add an elective to favorites
   */
  add: async (electiveId: string): Promise<void> => {
    const response = await fetchBackend(`/api/users/me/favorites/${electiveId}`, {
      method: "POST",
    });
    if (!response.ok) {
      throw new Error(`Failed to add favorite: ${response.statusText}`);
    }
  },

  /**
   * Remove an elective from favorites
   */
  remove: async (electiveId: string): Promise<void> => {
    const response = await fetchBackend(`/api/users/me/favorites/${electiveId}`, {
      method: "DELETE",
    });
    if (!response.ok) {
      throw new Error(`Failed to remove favorite: ${response.statusText}`);
    }
  },

  /**
   * Toggle favorite status for an elective
   */
  toggle: async (electiveId: string, isFavorited: boolean): Promise<void> => {
    if (isFavorited) {
      await favoritesApi.remove(electiveId);
    } else {
      await favoritesApi.add(electiveId);
    }
  },
};

// ============================================
// Auth API
// ============================================

export const authApi = {
  /**
   * Login with email and password
   */
  login: async (email: string, password: string): Promise<void> => {
    const response = await fetchBackend("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: email.trim(), password }),
    });

    if (!response.ok) {
      // Try to parse error message from response
      const raw = await (response as unknown as Response).text();
      let errorData: any = null;
      try {
        errorData = raw ? JSON.parse(raw) : null;
      } catch {
        errorData = { _raw: raw };
      }

      const backendMessage =
        errorData?.message || errorData?.error || errorData?._raw || response.statusText;

      if (response.status === 401) {
        throw new Error("Invalid credentials. Please check your email and password.");
      } else if (response.status === 400) {
        throw new Error(backendMessage || "Bad request. Please verify your input.");
      } else if (response.status >= 500) {
        throw new Error("Server error. Please try again later.");
      } else {
        throw new Error(backendMessage || `Request failed with status ${response.status}.`);
      }
    }
  },
};

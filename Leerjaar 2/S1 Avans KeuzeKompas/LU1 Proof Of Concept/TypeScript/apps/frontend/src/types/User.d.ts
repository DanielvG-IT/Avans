import type { Elective } from "./Elective";

export type UserRole = "student" | "teacher" | "admin";

export interface BaseUser {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  passwordHash: string;
  createdAt?: string;
  updatedAt?: string;
}

/** 👩‍🎓 Student */
export interface StudentUser extends BaseUser {
  role: "student";
  favorites: Elective[];
}

/** 👨‍🏫 Teacher */
export interface TeacherUser extends BaseUser {
  role: "teacher";
}

/** 🧑‍💼 Admin */
export interface AdminUser extends BaseUser {
  role: "admin";
}

/** 🎯 Discriminated Union */
export type User = StudentUser | TeacherUser | AdminUser;

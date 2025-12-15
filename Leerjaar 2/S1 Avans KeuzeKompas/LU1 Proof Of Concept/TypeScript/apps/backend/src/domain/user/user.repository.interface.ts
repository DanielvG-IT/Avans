import { User, StudentUser, TeacherUser, AdminUser } from "./user";

export interface IUserRepository {
  // Generic user operations
  find(): Promise<User[]>;
  findById(id: string): Promise<User | null>;
  findByEmail(email: string): Promise<User | null>;
  create(data: User): Promise<User>;
  update(id: string, data: User | Partial<User>): Promise<User | null>;
  delete(id: string): Promise<boolean>;

  // Role-specific finders
  findStudents(): Promise<StudentUser[]>;
  findTeachers(): Promise<TeacherUser[]>;
  findAdmins(): Promise<AdminUser[]>;
  findStudentById(id: string): Promise<StudentUser | null>;
  findTeacherById(id: string): Promise<TeacherUser | null>;
  findAdminById(id: string): Promise<AdminUser | null>;
}

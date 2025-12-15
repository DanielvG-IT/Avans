export type UserRole = 'STUDENT' | 'TEACHER' | 'ADMIN';

export interface User {
  id: string;
  name: string;
  email: string;
  hashedPassword: string;
  role: string;

  createdAt: Date;
  updatedAt: Date;
}

import type { Elective } from '@/types/Elective';
import type { StudentUser, TeacherUser, AdminUser } from '@/types/User';

export const mockElective: Elective = {
  id: '1',
  code: 'ELEC001',
  name: 'Advanced TypeScript',
  description: 'Learn advanced TypeScript patterns and best practices for modern web development.',
  provider: 'Technische Bedrijfskunde',
  period: 'P3',
  duration: '1 Periode',
  credits: 5,
  language: 'English',
  location: 'Breda',
  level: 'NLQF5',
  tags: ['programming', 'typescript', 'web'],
  teachers: [],
  createdAt: '2024-01-01T00:00:00.000Z',
  updatedAt: '2024-01-01T00:00:00.000Z',
};

export const mockElectives: Elective[] = [
  mockElective,
  {
    id: '2',
    code: 'ELEC002',
    name: 'React Development',
    description: 'Master React and its ecosystem for building modern web applications.',
    provider: 'Informatica',
    period: 'P4',
    duration: '1 Periode',
    credits: 5,
    language: 'Nederlands',
    location: 'Breda',
    level: 'NLQF6',
    tags: ['react', 'javascript', 'frontend'],
    teachers: [],
    createdAt: '2024-01-02T00:00:00.000Z',
    updatedAt: '2024-01-02T00:00:00.000Z',
  },
  {
    id: '3',
    code: 'ELEC003',
    name: 'Database Design',
    description: 'Learn database design principles and implementation strategies.',
    provider: 'Software Engineering',
    period: 'P1',
    duration: '2 Periode',
    credits: 10,
    language: 'English',
    location: 'Den Bosch',
    level: 'NLQF5',
    tags: ['database', 'sql', 'backend'],
    teachers: [],
    createdAt: '2024-01-03T00:00:00.000Z',
    updatedAt: '2024-01-03T00:00:00.000Z',
  },
];

export const mockStudentUser: StudentUser = {
  id: 'student-1',
  firstName: 'John',
  lastName: 'Doe',
  email: 'john.doe@example.com',
  role: 'student',
  passwordHash: 'hashed-password',
  favorites: [mockElective],
  createdAt: '2024-01-01T00:00:00.000Z',
  updatedAt: '2024-01-01T00:00:00.000Z',
};

export const mockTeacherUser: TeacherUser = {
  id: 'teacher-1',
  firstName: 'Jane',
  lastName: 'Smith',
  email: 'jane.smith@example.com',
  role: 'teacher',
  passwordHash: 'hashed-password',
  createdAt: '2024-01-01T00:00:00.000Z',
  updatedAt: '2024-01-01T00:00:00.000Z',
};

export const mockAdminUser: AdminUser = {
  id: 'admin-1',
  firstName: 'Admin',
  lastName: 'User',
  email: 'admin@example.com',
  role: 'admin',
  passwordHash: 'hashed-password',
  createdAt: '2024-01-01T00:00:00.000Z',
  updatedAt: '2024-01-01T00:00:00.000Z',
};

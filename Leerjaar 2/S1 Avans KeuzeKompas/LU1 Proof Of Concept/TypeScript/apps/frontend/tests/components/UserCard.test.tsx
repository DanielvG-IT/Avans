import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '../helpers/test-utils';
import userEvent from '@testing-library/user-event';
import { UserCard } from '@/components/users/UserCard';
import { mockStudentUser, mockTeacherUser } from '../helpers/mockData';

describe('UserCard', () => {
  const mockOnEdit = vi.fn();
  const mockOnDelete = vi.fn();

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('Student User', () => {
    it('renders student information correctly', () => {
      render(
        <UserCard
          user={mockStudentUser}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      expect(screen.getByText('John Doe')).toBeInTheDocument();
      expect(screen.getByText('john.doe@example.com')).toBeInTheDocument();
      expect(screen.getByText('👩‍🎓 Student')).toBeInTheDocument();
    });

    it('displays favorite electives for student', () => {
      render(
        <UserCard
          user={mockStudentUser}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      expect(screen.getByText(/Favorite Electives/)).toBeInTheDocument();
      expect(screen.getByText('Advanced TypeScript')).toBeInTheDocument();
    });

    it('does not show favorites section when student has no favorites', () => {
      const studentWithoutFavorites = {
        ...mockStudentUser,
        favorites: [],
      };
      render(
        <UserCard
          user={studentWithoutFavorites}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      expect(screen.queryByText(/Favorite Electives/)).not.toBeInTheDocument();
    });
  });

  describe('Teacher User', () => {
    it('renders teacher information correctly', () => {
      render(
        <UserCard
          user={mockTeacherUser}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      expect(screen.getByText('Jane Smith')).toBeInTheDocument();
      expect(screen.getByText('jane.smith@example.com')).toBeInTheDocument();
      expect(screen.getByText('👨‍🏫 Teacher')).toBeInTheDocument();
    });

    it('displays teacher-specific message', () => {
      render(
        <UserCard
          user={mockTeacherUser}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      expect(
        screen.getByText('View electives to see teaching assignments')
      ).toBeInTheDocument();
    });
  });

  describe('User Actions', () => {
    it('calls onEdit when Edit button is clicked', async () => {
      const user = userEvent.setup();
      render(
        <UserCard
          user={mockStudentUser}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      const editButton = screen.getByRole('button', { name: /edit/i });
      await user.click(editButton);

      expect(mockOnEdit).toHaveBeenCalledWith(mockStudentUser);
      expect(mockOnEdit).toHaveBeenCalledTimes(1);
    });

    it('calls onDelete when Delete button is clicked', async () => {
      const user = userEvent.setup();
      render(
        <UserCard
          user={mockStudentUser}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      const deleteButton = screen.getByRole('button', { name: /delete/i });
      await user.click(deleteButton);

      expect(mockOnDelete).toHaveBeenCalledWith('student-1', 'student');
      expect(mockOnDelete).toHaveBeenCalledTimes(1);
    });
  });

  describe('Created Date', () => {
    it('displays formatted join date when createdAt is present', () => {
      render(
        <UserCard
          user={mockStudentUser}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      expect(screen.getByText(/Joined:/)).toBeInTheDocument();
    });

    it('does not display join date when createdAt is missing', () => {
      const userWithoutDate = {
        ...mockStudentUser,
        createdAt: undefined,
      };
      render(
        <UserCard
          user={userWithoutDate}
          onEdit={mockOnEdit}
          onDelete={mockOnDelete}
        />
      );

      expect(screen.queryByText(/Joined:/)).not.toBeInTheDocument();
    });
  });
});

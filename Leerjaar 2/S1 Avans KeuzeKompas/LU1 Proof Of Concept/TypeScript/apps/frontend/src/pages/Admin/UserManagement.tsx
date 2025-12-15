import type { StudentUser, TeacherUser, UserRole } from "@/types/User";
import { UserFormDialog } from "@/components/users/UserFormDialog";
import { UserTabs } from "@/components/users/UserTabs";
import { UserList } from "@/components/users/UserList";
import ErrorState from "@/components/ErrorState";
import { userApi } from "@/services/api.service";
import { useState, useEffect } from "react";
import Loading from "@/components/Loading";
import { toast } from "sonner";
import {
  AlertDialogDescription,
  AlertDialogContent,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialog,
} from "@/components/ui/alert-dialog";

interface UserFormData {
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  role: UserRole;
}

export default function UserManagement() {
  const [students, setStudents] = useState<StudentUser[]>([]);
  const [teachers, setTeachers] = useState<TeacherUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<"students" | "teachers">("students");

  // Dialog state
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editingUserId, setEditingUserId] = useState<string | null>(null);
  const [formData, setFormData] = useState<UserFormData>({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    role: "student",
  });
  const [formSubmitting, setFormSubmitting] = useState(false);

  // Delete alert dialog state
  const [isDeleteAlertOpen, setIsDeleteAlertOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState<{
    id: string;
    role: "student" | "teacher";
  } | null>(null);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      setError(null);

      const users = await userApi.getAll();
      const { students: studentsList, teachers: teachersList } = userApi.separateByRole(users);

      setStudents(studentsList);
      setTeachers(teachersList);
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred");
      toast.error("Failed to load users");
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteUser = (userId: string, role: "student" | "teacher") => {
    setUserToDelete({ id: userId, role });
    setIsDeleteAlertOpen(true);
  };

  const confirmDeleteUser = async () => {
    if (!userToDelete) return;

    try {
      await userApi.delete(userToDelete.id);
      toast.success("User deleted successfully");

      // Update local state
      if (userToDelete.role === "student") {
        setStudents(students.filter((s) => s.id !== userToDelete.id));
      } else {
        setTeachers(teachers.filter((t) => t.id !== userToDelete.id));
      }
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed to delete user");
    } finally {
      setIsDeleteAlertOpen(false);
      setUserToDelete(null);
    }
  };

  const openCreateDialog = (role: "student" | "teacher") => {
    setIsEditing(false);
    setEditingUserId(null);
    setFormData({
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      role,
    });
    setIsDialogOpen(true);
  };

  const openEditDialog = (user: StudentUser | TeacherUser) => {
    setIsEditing(true);
    setEditingUserId(user.id!);
    setFormData({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      role: user.role,
    });
    setIsDialogOpen(true);
  };

  const handleFormSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormSubmitting(true);

    try {
      if (isEditing) {
        // Update existing user
        const updatedUser = await userApi.update(editingUserId!, {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
        });

        toast.success("User updated successfully");

        // Update local state
        if (updatedUser.role === "student") {
          setStudents(
            students.map((s) => (s.id === editingUserId ? (updatedUser as StudentUser) : s)),
          );
        } else if (updatedUser.role === "teacher") {
          setTeachers(
            teachers.map((t) => (t.id === editingUserId ? (updatedUser as TeacherUser) : t)),
          );
        }
      } else {
        // Create new user
        if (!formData.password) {
          toast.error("Password is required for new users");
          return;
        }

        const newUser = await userApi.create({
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          password: formData.password,
          role: formData.role,
        });

        toast.success("User created successfully");

        // Update local state
        if (newUser.role === "student") {
          setStudents([...students, newUser as StudentUser]);
        } else if (newUser.role === "teacher") {
          setTeachers([...teachers, newUser as TeacherUser]);
        }
      }

      setIsDialogOpen(false);
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "An error occurred");
    } finally {
      setFormSubmitting(false);
    }
  };

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <ErrorState error={error} />;
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-4xl font-bold mb-2">User Management</h1>
        <p className="text-muted-foreground">Manage students and teachers in the system</p>
      </div>

      <UserTabs
        activeTab={activeTab}
        onTabChange={setActiveTab}
        studentCount={students.length}
        teacherCount={teachers.length}
      />

      {activeTab === "students" && (
        <UserList
          users={students}
          userType="student"
          onAdd={() => openCreateDialog("student")}
          onEdit={openEditDialog}
          onDelete={handleDeleteUser}
        />
      )}

      {activeTab === "teachers" && (
        <UserList
          users={teachers}
          userType="teacher"
          onAdd={() => openCreateDialog("teacher")}
          onEdit={openEditDialog}
          onDelete={handleDeleteUser}
        />
      )}

      <UserFormDialog
        open={isDialogOpen}
        onOpenChange={setIsDialogOpen}
        isEditing={isEditing}
        formData={formData}
        onFormDataChange={setFormData}
        onSubmit={handleFormSubmit}
        submitting={formSubmitting}
      />

      <AlertDialog open={isDeleteAlertOpen} onOpenChange={setIsDeleteAlertOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete this user from the system.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDeleteUser}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}

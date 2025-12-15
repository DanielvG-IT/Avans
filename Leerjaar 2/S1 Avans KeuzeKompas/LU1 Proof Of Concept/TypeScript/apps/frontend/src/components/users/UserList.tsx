import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import type { StudentUser, TeacherUser } from "@/types/User";
import { UserCard } from "./UserCard";

interface UserListProps {
  users: StudentUser[] | TeacherUser[];
  userType: "student" | "teacher";
  onAdd: () => void;
  onEdit: (user: StudentUser | TeacherUser) => void;
  onDelete: (userId: string, role: "student" | "teacher") => void;
}

export function UserList({ users, userType, onAdd, onEdit, onDelete }: UserListProps) {
  const title = userType === "student" ? "Students" : "Teachers";
  const addButtonText = userType === "student" ? "Add Student" : "Add Teacher";
  const emptyMessage = `No ${userType}s found`;

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-2xl font-semibold">{title}</h2>
        <Button onClick={onAdd}>{addButtonText}</Button>
      </div>

      {users.length === 0 ? (
        <Card className="p-8 text-center">
          <p className="text-muted-foreground">{emptyMessage}</p>
        </Card>
      ) : (
        <div className="grid gap-4">
          {users.map((user) => (
            <UserCard key={user.id} user={user} onEdit={onEdit} onDelete={onDelete} />
          ))}
        </div>
      )}
    </div>
  );
}

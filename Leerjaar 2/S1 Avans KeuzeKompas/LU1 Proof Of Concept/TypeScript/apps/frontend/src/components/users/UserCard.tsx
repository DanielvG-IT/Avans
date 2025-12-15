import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import type { StudentUser, TeacherUser } from "@/types/User";

interface UserCardProps {
  user: StudentUser | TeacherUser;
  onEdit: (user: StudentUser | TeacherUser) => void;
  onDelete: (userId: string, role: "student" | "teacher") => void;
}

export function UserCard({ user, onEdit, onDelete }: UserCardProps) {
  const isStudent = user.role === "student";

  return (
    <Card className="p-6">
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <div className="flex items-center gap-3 mb-2">
            <h3 className="text-xl font-semibold">
              {user.firstName} {user.lastName}
            </h3>
            <Badge variant="secondary">{isStudent ? "👩‍🎓 Student" : "👨‍🏫 Teacher"}</Badge>
          </div>
          <p className="text-muted-foreground mb-3">{user.email}</p>

          {/* Student-specific: Favorites */}
          {isStudent && "favorites" in user && user.favorites && user.favorites.length > 0 && (
            <div className="mt-4">
              <p className="text-sm font-medium mb-2">
                Favorite Electives ({user.favorites.length}):
              </p>
              <div className="flex flex-wrap gap-2">
                {user.favorites.map((elective) => (
                  <Badge key={elective.id} variant="outline">
                    {elective.name}
                  </Badge>
                ))}
              </div>
            </div>
          )}

          {/* Teacher-specific: Note about modules */}
          {!isStudent && user.role === "teacher" && (
            <div className="mt-4">
              <p className="text-sm text-muted-foreground">
                View electives to see teaching assignments
              </p>
            </div>
          )}

          {user.createdAt && (
            <p className="text-xs text-muted-foreground mt-3">
              Joined: {new Date(user.createdAt).toLocaleDateString()}
            </p>
          )}
        </div>

        <div className="flex gap-2 ml-4">
          <Button variant="outline" size="sm" onClick={() => onEdit(user)}>
            Edit
          </Button>
          <Button variant="destructive" size="sm" onClick={() => onDelete(user.id!, user.role)}>
            Delete
          </Button>
        </div>
      </div>
    </Card>
  );
}

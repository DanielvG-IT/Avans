import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import type { User, StudentUser, TeacherUser, AdminUser } from "@/types/User";
import { userApi, favoritesApi } from "@/services/api.service";
import { useCallback, useEffect, useState } from "react";
import { Toaster } from "@/components/ui/sonner";
import ErrorState from "@/components/ErrorState";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import Loading from "@/components/Loading";
import { useElectives } from "@/hooks/useElectives";
import Cookies from "js-cookie";
import { toast } from "sonner";

const ProfilePage = () => {
  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const loadProfile = useCallback(async () => {
    try {
      setError(null);
      setLoading(true);

      // Fetch user data
      const userData = await userApi.getMe();

      let data: User = userData;

      // Enrich data based on user role
      if (userData.role === "student") {
        const favoritesData = await favoritesApi.getAll();
        data = {
          ...userData,
          favorites: favoritesData || [],
        } as StudentUser;
      }

      setUser(data);
    } catch (err: any) {
      console.error(err);
      const errorMessage = err?.message ?? "An unexpected error occurred while loading profile.";
      setError(errorMessage);
      toast.error(errorMessage);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadProfile();
  }, [loadProfile]);

  if (loading) return <Loading isFullScreen={false} showLogo={false} />;

  if (error)
    return (
      <>
        <Toaster />
        <div
          role="status"
          aria-live="polite"
          className="min-h-[60vh] flex flex-col items-center justify-center"
        >
          <ErrorState error={error} />
          <div className="mt-4">
            <Button onClick={() => void loadProfile()} variant="outline" className="rounded-xl">
              Retry
            </Button>
          </div>
        </div>
      </>
    );

  if (!user)
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh] text-center p-6">
        <p className="text-muted-foreground mb-4">Profile not found or session expired.</p>
        <div className="flex gap-2">
          <Button onClick={() => loadProfile()} variant="outline" className="rounded-xl">
            Retry
          </Button>
          <Button
            onClick={() => (window.location.href = "/auth/login")}
            variant="destructive"
            className="rounded-xl"
          >
            Go to Login
          </Button>
        </div>
      </div>
    );

  return (
    <div className="max-w-6xl mx-auto px-4 py-10 space-y-8">
      <Toaster />

      {/* HEADER */}
      <div className="flex flex-col sm:flex-row items-center sm:justify-between gap-6">
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 rounded-full bg-gradient-to-br from-primary to-primary/70 text-white flex items-center justify-center text-2xl font-bold shadow-md">
            {user.firstName.charAt(0).toUpperCase()}
          </div>
          <div>
            <h1 className="text-2xl font-bold">
              {user.firstName} {user.lastName}
            </h1>
            <p className="text-muted-foreground text-sm">{user.email}</p>
          </div>
        </div>

        <div className="flex">
          <Button
            variant="destructive"
            className="rounded-xl"
            onClick={() => {
              toast.success("Logged out!");
              Cookies.remove("ACCESSTOKEN", { path: "/" });
              window.location.replace("/auth/login");
            }}
          >
            Log Out
          </Button>
        </div>
      </div>

      {/* INFO CARD */}
      <Card className="border border-border/40 bg-gradient-to-b from-background to-muted/10 rounded-3xl shadow-sm">
        <CardHeader>
          <CardTitle className="text-lg">Account Information</CardTitle>
          <CardDescription>Basic info about your account</CardDescription>
        </CardHeader>
        <CardContent className="grid grid-cols-1 sm:grid-cols-2 gap-y-3 text-sm">
          <div>
            <span className="font-medium text-foreground">Name:</span> {user.firstName}{" "}
            {user.lastName}
          </div>
          <div>
            <span className="font-medium text-foreground">Email:</span> {user.email}
          </div>
          <div>
            <span className="font-medium text-foreground">Joined:</span>{" "}
            {user.createdAt ? new Date(user.createdAt).toLocaleDateString() : "Unknown"}
          </div>
        </CardContent>
      </Card>

      {/* FAVORITES (STUDENT ONLY) */}
      {user.role === "student" && <StudentProfile user={user as StudentUser} />}

      {/* MODULES GIVEN (TEACHER ONLY) */}
      {user.role === "teacher" && <TeacherProfile user={user as TeacherUser} />}

      {/* ADMIN INFO */}
      {user.role === "admin" && <AdminProfile user={user as AdminUser} />}
    </div>
  );
};

export default ProfilePage;

// ============================================================================
// 👩‍🎓 STUDENT PROFILE COMPONENT
// ============================================================================
const StudentProfile = ({ user }: { user: StudentUser }) => (
  <div className="space-y-4">
    <h2 className="text-xl font-semibold">⭐ Favorited Electives</h2>

    {(!user.favorites || user.favorites.length === 0) && (
      <Card className="rounded-2xl border border-border/40 bg-gradient-to-b from-background to-muted/10">
        <CardHeader>
          <CardTitle className="text-lg">No favorites yet</CardTitle>
          <CardDescription>Save electives you're interested in for quick access.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col items-start sm:flex-row sm:items-center gap-4">
          <div className="flex-1 text-sm text-muted-foreground">
            You haven't favorited any electives. Browse electives to find ones you like and mark
            them as favorites.
          </div>
          <div className="flex gap-2">
            <Button
              onClick={() => (window.location.href = "/electives")}
              variant="outline"
              className="rounded-xl"
            >
              Browse Electives
            </Button>
          </div>
        </CardContent>
      </Card>
    )}

    <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {user.favorites?.map((elective) => (
        <Card
          key={elective.id}
          className="rounded-2xl border border-border/40 hover:border-primary/50 transition-all hover:shadow-md"
        >
          <CardHeader>
            <CardTitle className="text-base line-clamp-1">{elective.name}</CardTitle>
            <CardDescription className="text-xs text-muted-foreground">
              {elective.code} • {elective.provider}
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <p className="text-muted-foreground line-clamp-3">
              {elective.description || "No description available."}
            </p>
            <div className="flex flex-wrap gap-1 pt-2">
              {elective.language && (
                <Badge variant="secondary" className="text-xs">
                  {elective.language}
                </Badge>
              )}
              {elective.credits && (
                <Badge variant="outline" className="text-xs">
                  {elective.credits} EC
                </Badge>
              )}
              {elective.period && (
                <Badge variant="secondary" className="text-xs">
                  {elective.period}
                </Badge>
              )}
            </div>
            <Button
              variant="outline"
              className="w-full mt-2"
              onClick={() => (window.location.href = `/electives/${elective.id}`)}
            >
              View Details →
            </Button>
          </CardContent>
        </Card>
      ))}
    </div>
  </div>
);

// ============================================================================
// 👨‍🏫 TEACHER PROFILE COMPONENT
// ============================================================================
const TeacherProfile = ({ user }: { user: TeacherUser }) => {
  const { electives, loading } = useElectives();

  // Filter electives where this teacher is assigned
  const teacherElectives = (electives || []).filter((elective) =>
    elective.teachers?.some((teacher) => teacher.id === user.id),
  );

  if (loading) {
    return <Loading />;
  }

  return (
    <div className="space-y-4">
      <h2 className="text-xl font-semibold">📚 Modules Teaching</h2>

      {teacherElectives.length === 0 && (
        <Card className="rounded-2xl border border-border/40 bg-gradient-to-b from-background to-muted/10">
          <CardHeader>
            <CardTitle className="text-lg">No modules assigned</CardTitle>
            <CardDescription>You are not currently teaching any modules.</CardDescription>
          </CardHeader>
          <CardContent className="flex flex-col items-start sm:flex-row sm:items-center gap-4">
            <div className="flex-1 text-sm text-muted-foreground">
              Contact an administrator to get assigned to modules.
            </div>
          </CardContent>
        </Card>
      )}

      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {teacherElectives.map((elective) => (
          <Card
            key={elective.id}
            className="rounded-2xl border border-border/40 hover:border-primary/50 transition-all hover:shadow-md"
          >
            <CardHeader>
              <CardTitle className="text-base line-clamp-1">{elective.name}</CardTitle>
              <CardDescription className="text-xs text-muted-foreground">
                {elective.code} • {elective.provider}
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-2 text-sm">
              <p className="text-muted-foreground line-clamp-3">
                {elective.description || "No description available."}
              </p>
              <div className="flex flex-wrap gap-1 pt-2">
                {elective.language && (
                  <Badge variant="secondary" className="text-xs">
                    {elective.language}
                  </Badge>
                )}
                {elective.location && (
                  <Badge variant="outline" className="text-xs">
                    {elective.location}
                  </Badge>
                )}
                {elective.credits && (
                  <Badge variant="outline" className="text-xs">
                    {elective.credits} EC
                  </Badge>
                )}
                {elective.period && (
                  <Badge variant="secondary" className="text-xs">
                    {elective.period}
                  </Badge>
                )}
              </div>
              <Button
                variant="outline"
                className="w-full mt-2"
                onClick={() => (window.location.href = `/electives/${elective.id}`)}
              >
                View Details →
              </Button>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
};

// ============================================================================
// 🧑‍💼 ADMIN PROFILE COMPONENT
// ============================================================================
const AdminProfile = ({ user: _user }: { user: AdminUser }) => (
  <div className="space-y-4">
    <h2 className="text-xl font-semibold">⚙️ Admin Tools</h2>

    <Card className="rounded-2xl border border-border/40 bg-gradient-to-b from-background to-muted/10">
      <CardHeader>
        <CardTitle className="text-lg">Admin Access</CardTitle>
        <CardDescription>You have administrative privileges on this platform.</CardDescription>
      </CardHeader>
      <CardContent className="space-y-3">
        <p className="text-sm text-muted-foreground">
          As an administrator, you have full access to manage users, electives, and system settings.
        </p>
        <div className="flex flex-wrap gap-2">
          <Button
            onClick={() => (window.location.href = "/admin")}
            variant="default"
            className="rounded-xl"
          >
            Go to Admin Panel →
          </Button>
        </div>
      </CardContent>
    </Card>
  </div>
);

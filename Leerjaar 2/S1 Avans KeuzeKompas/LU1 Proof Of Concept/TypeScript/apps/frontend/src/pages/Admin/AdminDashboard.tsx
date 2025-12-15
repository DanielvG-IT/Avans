/**
 * Admin Dashboard - Only accessible by teachers and admins
 */

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { RoleProtected } from "@/components/auth/RoleProtected";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/hooks/useAuth";

const AdminDashboard = () => {
  const { user } = useAuth();

  return (
    <div className="max-w-7xl mx-auto px-4 py-10 space-y-8">
      <div className="space-y-2">
        <h1 className="text-3xl font-bold text-foreground">Admin Dashboard</h1>
        <p className="text-muted-foreground">
          Welcome, {user?.firstName}! Your role: <span className="font-semibold">{user?.role}</span>
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {/* Teacher and Admin Features */}
        <RoleProtected allowedRoles={["teacher", "admin"]}>
          <Card>
            <CardHeader>
              <CardTitle>Manage Electives</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground mb-4">
                Create, edit, and manage elective courses
              </p>
              <Button className="w-full">Manage Electives</Button>
            </CardContent>
          </Card>
        </RoleProtected>

        {/* Admin Only Features */}
        <RoleProtected allowedRoles="admin">
          <Card className="border-destructive/50">
            <CardHeader>
              <CardTitle>User Management</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground mb-4">Manage users, roles, and permissions</p>
              <Button variant="destructive" className="w-full">
                Manage Users
              </Button>
            </CardContent>
          </Card>
        </RoleProtected>

        <RoleProtected allowedRoles="admin">
          <Card className="border-destructive/50">
            <CardHeader>
              <CardTitle>System Settings</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground mb-4">Configure system-wide settings</p>
              <Button variant="outline" className="w-full">
                Settings
              </Button>
            </CardContent>
          </Card>
        </RoleProtected>

        {/* Teacher Features */}
        <RoleProtected allowedRoles="teacher">
          <Card>
            <CardHeader>
              <CardTitle>My Courses</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground mb-4">View and manage your courses</p>
              <Button variant="outline" className="w-full">
                View Courses
              </Button>
            </CardContent>
          </Card>
        </RoleProtected>

        {/* Reports - Available to both */}
        <RoleProtected allowedRoles={["teacher", "admin"]}>
          <Card>
            <CardHeader>
              <CardTitle>Reports & Analytics</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground mb-4">View enrollment and performance data</p>
              <Button variant="outline" className="w-full">
                View Reports
              </Button>
            </CardContent>
          </Card>
        </RoleProtected>
      </div>

      {/* Role-specific messages */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <RoleProtected
          allowedRoles="admin"
          fallback={
            <Card className="bg-muted/30">
              <CardContent className="pt-6">
                <p className="text-sm text-muted-foreground">
                  You need admin privileges to access advanced system features.
                </p>
              </CardContent>
            </Card>
          }
        >
          <Card className="bg-destructive/10 border-destructive/50">
            <CardContent className="pt-6">
              <p className="text-sm font-semibold text-destructive">
                ⚠️ Admin Access: You have full system control. Use with caution.
              </p>
            </CardContent>
          </Card>
        </RoleProtected>

        <RoleProtected allowedRoles="teacher">
          <Card className="bg-primary/10 border-primary/50">
            <CardContent className="pt-6">
              <p className="text-sm font-semibold text-primary">
                📚 Teacher Access: You can manage your courses and view student data.
              </p>
            </CardContent>
          </Card>
        </RoleProtected>
      </div>
    </div>
  );
};

export default AdminDashboard;

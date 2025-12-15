import RecommendationsPage from "./pages/Recommendations/Recommendations";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import ElectiveDetailPage from "./pages//Electives/ElectiveDetails";
import { ThemeProvider } from "@/components/ThemeProvider";
import ProtectedRoute from "@/components/auth/ProtectedRoute";
import ElectivesPage from "./pages/Electives/Electives";
import { AuthLayout } from "@/layouts/AuthLayout";
import ProfilePage from "@/pages/Auth/Profile";
import { createRoot } from "react-dom/client";
import LoginPage from "@/pages/Auth/Login";
import { Layout } from "@/layouts/Layout";
import NotFound from "@/pages/NotFound";
import HomePage from "@/pages/Home";
import { StrictMode } from "react";
import "@/main.css";
import { RoleProtectedRoute } from "./components/auth/RoleProtectedRoute";
import AdminDashboard from "./pages/Admin/AdminDashboard";
import UserManagement from "./pages/Admin/UserManagement";

const router = createBrowserRouter([
  {
    // Main app layout (protected)
    element: <Layout />,
    children: [
      {
        path: "/",
        element: (
          <ProtectedRoute>
            <HomePage />
          </ProtectedRoute>
        ),
      },
      {
        path: "/electives",
        element: (
          <ProtectedRoute>
            <ElectivesPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "/electives/:electiveId",
        element: (
          <ProtectedRoute>
            <ElectiveDetailPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "/recommendations",
        element: (
          <ProtectedRoute>
            <RoleProtectedRoute allowedRoles={["student"]}>
              <RecommendationsPage />
            </RoleProtectedRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "/users",
        element: (
          <ProtectedRoute>
            <RoleProtectedRoute allowedRoles={["admin"]}>
              <UserManagement />
            </RoleProtectedRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "/admin",
        element: (
          <ProtectedRoute>
            <RoleProtectedRoute allowedRoles={["admin"]}>
              <AdminDashboard />
            </RoleProtectedRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "/profile",
        element: (
          <ProtectedRoute>
            <ProfilePage />
          </ProtectedRoute>
        ),
      },
    ],
  },
  {
    // Auth layout (login/register pages)
    element: <AuthLayout />,
    children: [{ path: "/auth/login", element: <LoginPage /> }],
  },
  // fallback for unknown routes
  { path: "*", element: <NotFound /> },
]);

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider
      defaultTheme="system"
      storageKey="theme"
      enableSystem
      disableTransitionOnChange={false}
    >
      <RouterProvider router={router} />
    </ThemeProvider>
  </StrictMode>,
);

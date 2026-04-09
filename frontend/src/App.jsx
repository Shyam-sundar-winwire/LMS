import { Navigate, Route, Routes } from "react-router-dom";
import { AppShell } from "./components/AppShell";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { useAuth } from "./context/AuthContext";
import { AllLeavesPage } from "./pages/AllLeavesPage";
import { ApplyLeavePage } from "./pages/ApplyLeavePage";
import { ApprovalsPage } from "./pages/ApprovalsPage";
import { DashboardPage } from "./pages/DashboardPage";
import { LoginPage } from "./pages/LoginPage";
import { MyLeavesPage } from "./pages/MyLeavesPage";
import { NotFoundPage } from "./pages/NotFoundPage";
import { ROLES } from "./utils/constants";

const ProtectedLayout = ({ children, roles }) => (
  <ProtectedRoute roles={roles}>
    <AppShell>{children}</AppShell>
  </ProtectedRoute>
);

const LoginGuard = () => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <Navigate to="/" replace /> : <LoginPage />;
};

const sharedRoles = [ROLES.EMPLOYEE, ROLES.MANAGER, ROLES.HR, ROLES.ADMIN];
const approvalRoles = [ROLES.MANAGER, ROLES.HR, ROLES.ADMIN];
const allLeaveRoles = [ROLES.HR, ROLES.ADMIN];

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginGuard />} />
      <Route
        path="/"
        element={
          <ProtectedLayout roles={sharedRoles}>
            <DashboardPage />
          </ProtectedLayout>
        }
      />
      <Route
        path="/apply"
        element={
          <ProtectedLayout roles={sharedRoles}>
            <ApplyLeavePage />
          </ProtectedLayout>
        }
      />
      <Route
        path="/my-leaves"
        element={
          <ProtectedLayout roles={sharedRoles}>
            <MyLeavesPage />
          </ProtectedLayout>
        }
      />
      <Route
        path="/approvals"
        element={
          <ProtectedLayout roles={approvalRoles}>
            <ApprovalsPage />
          </ProtectedLayout>
        }
      />
      <Route
        path="/all-leaves"
        element={
          <ProtectedLayout roles={allLeaveRoles}>
            <AllLeavesPage />
          </ProtectedLayout>
        }
      />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

export default App;

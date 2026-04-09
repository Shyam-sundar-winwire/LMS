import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { LoadingScreen } from "./LoadingScreen";

export const ProtectedRoute = ({ children, roles }) => {
  const { isAuthenticated, user, loading } = useAuth();

  if (loading) {
    return <LoadingScreen message="Checking your session..." />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (roles && !roles.includes(user.role)) {
    return <Navigate to="/" replace />;
  }

  return children;
};

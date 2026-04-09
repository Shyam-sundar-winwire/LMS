import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { authService } from "../services/authService";
import { storage } from "../utils/storage";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(storage.getUser());
  const [token, setToken] = useState(storage.getToken());
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!token || !user) {
      storage.clearSession();
    }
  }, [token, user]);

  const login = async (email, password) => {
    setLoading(true);
    try {
      const response = await authService.login({ email, password });
      setToken(response.token);
      setUser(response.user);
      storage.setToken(response.token);
      storage.setUser(response.user);
      return response.user;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    storage.clearSession();
  };

  const value = useMemo(
    () => ({
      user,
      token,
      loading,
      isAuthenticated: Boolean(token && user),
      login,
      logout
    }),
    [loading, token, user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  return context;
};

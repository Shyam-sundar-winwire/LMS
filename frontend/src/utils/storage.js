const TOKEN_KEY = "leaveapp.token";
const USER_KEY = "leaveapp.user";
const SEEDED_USER_NAMES = {
  "admin@leaveapp.com": "Shyam sundar",
  "manager@leaveapp.com": "Chaithanya",
  "hr@leaveapp.com": "Nadia",
  "employee@leaveapp.com": "Ravi"
};

const normalizeUser = (user) => {
  if (!user?.email) {
    return user;
  }

  const expectedFullName = SEEDED_USER_NAMES[user.email.toLowerCase()];
  if (!expectedFullName || user.fullName === expectedFullName) {
    return user;
  }

  return {
    ...user,
    fullName: expectedFullName
  };
};

export const storage = {
  getToken: () => localStorage.getItem(TOKEN_KEY),
  setToken: (token) => localStorage.setItem(TOKEN_KEY, token),
  clearToken: () => localStorage.removeItem(TOKEN_KEY),
  getUser: () => {
    const value = localStorage.getItem(USER_KEY);
    if (!value) {
      return null;
    }

    const normalizedUser = normalizeUser(JSON.parse(value));
    localStorage.setItem(USER_KEY, JSON.stringify(normalizedUser));
    return normalizedUser;
  },
  setUser: (user) => localStorage.setItem(USER_KEY, JSON.stringify(normalizeUser(user))),
  clearUser: () => localStorage.removeItem(USER_KEY),
  clearSession: () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }
};

import api from "./api";

export const dashboardService = {
  getSummary: async () => (await api.get("/dashboard/summary")).data
};

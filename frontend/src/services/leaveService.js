import api from "./api";

export const leaveService = {
  getMyLeaves: async () => (await api.get("/leaverequests/mine")).data,
  getAllLeaves: async () => (await api.get("/leaverequests/all")).data,
  getApprovals: async () => (await api.get("/leaverequests/approvals")).data,
  getBalances: async () => (await api.get("/leaverequests/balances")).data,
  getLeaveTypes: async () => (await api.get("/leaverequests/types")).data,
  applyLeave: async (payload) => (await api.post("/leaverequests", payload)).data,
  reviewLeave: async (id, payload) => (await api.put(`/leaverequests/${id}/review`, payload)).data
};

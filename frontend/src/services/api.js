import axios from "axios";
import { storage } from "../utils/storage";

const normalizeBaseUrl = (url) => url?.replace(/\/+$/, "");

const getCandidateBaseUrls = () => {
  const configuredBaseUrl = normalizeBaseUrl(import.meta.env.VITE_API_BASE_URL);
  const hostname = typeof window !== "undefined" ? window.location.hostname || "localhost" : "localhost";

  return [
    configuredBaseUrl,
    `http://${hostname}:5163/api`,
    `https://${hostname}:7057/api`,
    "http://localhost:5163/api",
    "https://localhost:7057/api"
  ].filter((url, index, urls) => Boolean(url) && urls.indexOf(url) === index);
};

const candidateBaseUrls = getCandidateBaseUrls();

const api = axios.create({
  baseURL: candidateBaseUrls[0],
  timeout: 15000
});

api.interceptors.request.use((config) => {
  const token = storage.getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const { config } = error;

    if (!error.response && config) {
      const triedBaseUrls = config.__triedBaseUrls ?? [config.baseURL ?? api.defaults.baseURL];
      const nextBaseUrl = candidateBaseUrls.find((url) => !triedBaseUrls.includes(url));

      if (nextBaseUrl) {
        config.__triedBaseUrls = [...triedBaseUrls, nextBaseUrl];
        config.baseURL = nextBaseUrl;
        return api.request(config);
      }
    }

    if (error.response?.status === 401) {
      storage.clearSession();
    }

    if (!error.response) {
      error.userMessage = `Unable to reach the API. Tried: ${candidateBaseUrls.join(", ")}.`;
    } else {
      error.userMessage = error.response.data?.message || "Something went wrong while processing your request.";
    }

    return Promise.reject(error);
  }
);

export default api;

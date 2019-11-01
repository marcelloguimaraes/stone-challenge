import axios from "axios";
import router from "../router";

const api = axios.create({
  baseURL: process.env.VUE_APP_ROOT_API
});

api.interceptors.response.use(null, error => {
  if (error.response.status === 401 || error.response.status === 403) {
    localStorage.removeItem("token");
    localStorage.removeItem("userId");
    localStorage.removeItem("email");

    return router.push({ name: "login" });
  }
  return Promise.reject(error);
});

export default api;

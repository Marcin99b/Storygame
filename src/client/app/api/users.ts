import { api } from "./client";
import type { UserProfile } from "./types";

export const usersApi = {
  me: () => api.get<UserProfile>("/users/Me"),
  login: () => api.post<void>("/users/Login"),
  logout: () => api.post<void>("/users/Logout"),
};

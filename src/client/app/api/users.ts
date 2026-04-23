import { api, resetCsrfToken } from "./client";
import type { UserProfile } from "./types";

export interface RegisterRequest {
  name: string;
  email: string;
}

export interface VerifyRequest {
  email: string;
  verificationCode: string;
}

export interface LoginRequest {
  email: string;
}

export interface ConfirmLoginRequest {
  loginConfirmationKey: string;
}

export const usersApi = {
  me: () => api.get<UserProfile>("/users/Me"),
  register: (body: RegisterRequest) => api.post<void>("/users/Register", body),
  verify: (body: VerifyRequest) => api.post<void>("/users/Verify", body),
  login: (body: LoginRequest) => api.post<void>("/users/Login", body),
  confirmLogin: async (body: ConfirmLoginRequest) => {
    await api.post<void>("/users/ConfirmLogin", body);
    resetCsrfToken();
  },
  logout: async () => {
    await api.post<void>("/users/Logout");
    resetCsrfToken();
  },
};

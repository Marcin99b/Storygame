import { api } from "./client";
import type { MailMessage } from "./types";

export const mailApi = {
  getMessages: (email: string) =>
    api.get<MailMessage[]>(`/mail/${encodeURIComponent(email)}`),
};

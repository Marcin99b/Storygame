import { api } from "./client";

export const trackingApi = {
  startTracking: (libraryBookId: string, totalLength: number) =>
    api.post<void>(`/tracking/${libraryBookId}`, { totalLength }),
};

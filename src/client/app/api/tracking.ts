import { api } from "./client";
import type { Tracking } from "./types";

export interface GetTrackingsResult {
  trackings: Tracking[];
}

export const trackingApi = {
  getTrackings: () => api.get<GetTrackingsResult>("/tracking/"),
  startTracking: (libraryBookId: string, totalLength: number) =>
    api.post<void>("/tracking/", { libraryBookId, totalLength }),
  updateIndex: (trackingId: string, newIndex: number) =>
    api.post<void>(`/tracking/${trackingId}/index`, { newIndex }),
};

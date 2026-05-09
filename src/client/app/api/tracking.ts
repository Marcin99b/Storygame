import { api } from "./client";
import type { TimePeriod, Tracking, TrackingStatistic } from "./types";

export interface GetTrackingsResult {
  trackings: Tracking[];
}

export interface GetStatisticsResult {
  trackingStatistics: TrackingStatistic[];
}

export const trackingApi = {
  getTrackings: () => api.get<GetTrackingsResult>("/tracking/"),
  startTracking: (libraryBookId: string) =>
    api.post<void>("/tracking/", { libraryBookId }),
  updateIndex: (trackingId: string, newIndex: number) =>
    api.post<void>(`/tracking/${trackingId}/index`, { newIndex }),
  getStatistics: (
    trackingId: string,
    timePeriod: TimePeriod,
    fromDateTime: string,
    toDateTime: string
  ) => {
    const params = new URLSearchParams({ timePeriod, fromDateTime, toDateTime });
    return api.get<GetStatisticsResult>(`/tracking/${trackingId}/stats?${params}`);
  },
};

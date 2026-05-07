using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking;

public interface ITrackingRepository
{
    Task AddTracking(Tracking tracking, CancellationToken ct);
    Task UpdateTracking(Tracking tracking, CancellationToken ct);
    Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId, CancellationToken ct);
    Task<IEnumerable<Tracking>> GetUserTrackings(Guid userId, CancellationToken ct);
    Task<Tracking> GetTracking(Guid trackingId, CancellationToken ct);
    Task AddStatistic(TrackingStatistic trackingStatistic, CancellationToken ct);
    Task UpdateStatistic(TrackingStatistic trackingStatistic, CancellationToken ct);
    Task<IEnumerable<TrackingStatistic>> GetStatistics(TimeRange timeRange, TimePeriod timePeriod, CancellationToken ct);
}
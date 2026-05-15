using Storygame.Tracking;
using System.Collections.Concurrent;
using TrackingEntity = Storygame.Tracking.Tracking;

namespace Storygame.Tests.Integration;

internal class InMemoryTrackingRepository : ITrackingRepository
{
    private readonly ConcurrentDictionary<Guid, TrackingEntity> _trackings = new();
    private readonly ConcurrentDictionary<Guid, TrackingStatistic> _statistics = new();

    public Task AddTracking(TrackingEntity tracking, CancellationToken ct)
    {
        _trackings[tracking.Id] = tracking;
        return Task.CompletedTask;
    }

    public Task UpdateTracking(TrackingEntity tracking, CancellationToken ct)
    {
        _trackings[tracking.Id] = tracking;
        return Task.CompletedTask;
    }

    public Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId, CancellationToken ct)
        => Task.FromResult(_trackings.Values.Any(t => t.LibraryBookId == libraryBookId));

    public Task<IEnumerable<TrackingEntity>> GetUserTrackings(Guid userId, CancellationToken ct)
        => Task.FromResult(_trackings.Values.Where(t => t.UserId == userId));

    public Task<TrackingEntity> GetTracking(Guid trackingId, CancellationToken ct)
        => Task.FromResult(_trackings[trackingId]);

    public Task AddStatistic(TrackingStatistic trackingStatistic, CancellationToken ct)
    {
        _statistics[trackingStatistic.Id] = trackingStatistic;
        return Task.CompletedTask;
    }

    public Task UpdateStatistic(TrackingStatistic trackingStatistic, CancellationToken ct)
    {
        _statistics[trackingStatistic.Id] = trackingStatistic;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<TrackingStatistic>> GetStatistics(Guid trackingId, TimeRange timeRange, TimePeriod timePeriod, CancellationToken ct)
    {
        var stats = _statistics.Values.Where(s =>
            s.TrackingId == trackingId &&
            s.TimePeriod == timePeriod &&
            s.TimeRange.From >= timeRange.From &&
            s.TimeRange.To <= timeRange.To);
        return Task.FromResult(stats);
    }

    public Task<TrackingStatistic?> GetStatisticByTimePoint(Guid trackingId, DateTime timePoint, TimePeriod timePeriod, CancellationToken ct)
    {
        var stat = _statistics.Values.FirstOrDefault(s =>
            s.TrackingId == trackingId &&
            s.TimePeriod == timePeriod &&
            s.TimeRange.From <= timePoint &&
            s.TimeRange.To >= timePoint);
        return Task.FromResult(stat);
    }
}

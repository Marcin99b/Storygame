using Storygame.Cqrs;
using Storygame.Tracking.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Projectors;

public class DailyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}

public class WeeklyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}

public class MonthlyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}

public class YearlyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
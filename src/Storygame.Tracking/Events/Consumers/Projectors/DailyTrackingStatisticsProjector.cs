using Storygame.Cqrs;
using Storygame.Tracking.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Storygame.Tracking.Events.Consumers.Projectors;

public class DailyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public async Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        var increment = @event.NewIndex - @event.OldIndex;

        var stat = await trackingRepository.GetStatisticByTimePoint(@event.TrackingId, @event.CreatedAt, TimePeriod.Day, ct);
        if (stat == null)
        {
            await trackingRepository.AddStatistic(new TrackingStatistic()
            {
                Id = Guid.NewGuid(),
                TrackingId = @event.TrackingId,
                TimePeriod = TimePeriod.Day,
                TimeRange = new TimeRange(@event.CreatedAt.Date, @event.CreatedAt.Date.AddDays(1).AddSeconds(-1)),
                Value = increment
            }, ct);
        }
        else
        {
            stat.Value += increment;
            await trackingRepository.UpdateStatistic(stat, ct);
        }
    }
}

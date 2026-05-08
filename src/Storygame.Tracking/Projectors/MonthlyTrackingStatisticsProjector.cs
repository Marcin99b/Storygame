using Storygame.Cqrs;
using Storygame.Tracking.Events;

namespace Storygame.Tracking.Projectors;

public class MonthlyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public async Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        var increment = @event.NewIndex - @event.OldIndex;

        var created = @event.CreatedAt.Date;
        var monthStart = new DateTime(created.Year, created.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

        var stat = await trackingRepository.GetStatisticByTimePoint(@event.TrackingId, @event.CreatedAt, TimePeriod.Month, ct);
        if (stat == null)
        {
            await trackingRepository.AddStatistic(new TrackingStatistic()
            {
                Id = Guid.NewGuid(),
                TrackingId = @event.TrackingId,
                TimePeriod = TimePeriod.Month,
                TimeRange = new TimeRange(monthStart, monthEnd),
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

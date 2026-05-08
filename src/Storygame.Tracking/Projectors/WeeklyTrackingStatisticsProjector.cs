using Storygame.Cqrs;
using Storygame.Tracking.Events;

namespace Storygame.Tracking.Projectors;

public class WeeklyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public async Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        var increment = @event.NewIndex - @event.OldIndex;

        var date = @event.CreatedAt.Date;
        int diff = (int)date.DayOfWeek - (int)DayOfWeek.Monday;
        if (diff < 0) diff += 7;
        var weekStart = date.AddDays(-diff);
        var weekEnd = weekStart.AddDays(7).AddSeconds(-1);

        var stat = await trackingRepository.GetStatisticByTimePoint(@event.TrackingId, @event.CreatedAt, TimePeriod.Week, ct);
        if (stat == null)
        {
            await trackingRepository.AddStatistic(new TrackingStatistic()
            {
                Id = Guid.NewGuid(),
                TrackingId = @event.TrackingId,
                TimePeriod = TimePeriod.Week,
                TimeRange = new TimeRange(weekStart, weekEnd),
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

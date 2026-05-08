using Storygame.Cqrs;
using Storygame.Tracking.Events;

namespace Storygame.Tracking.Projectors;

public class YearlyTrackingStatisticsProjector(ITrackingRepository trackingRepository) : IEventHandler<TrackingIndexUpdatedEvent>
{
    public async Task HandleAsync(TrackingIndexUpdatedEvent @event, CancellationToken ct)
    {
        var increment = @event.NewIndex - @event.OldIndex;

        var created = @event.CreatedAt.Date;
        var yearStart = new DateTime(created.Year, 1, 1);
        var yearEnd = yearStart.AddYears(1).AddSeconds(-1);

        var stat = await trackingRepository.GetStatisticByTimePoint(@event.TrackingId, @event.CreatedAt, TimePeriod.Year, ct);
        if (stat == null)
        {
            await trackingRepository.AddStatistic(new TrackingStatistic()
            {
                Id = Guid.NewGuid(),
                TrackingId = @event.TrackingId,
                TimePeriod = TimePeriod.Year,
                TimeRange = new TimeRange(yearStart, yearEnd),
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
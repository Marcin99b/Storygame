using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Queries;

public record GetTrackingStatisticsQuery(Guid TrackingId, TimeRange TimeRange, TimePeriod TimePeriod) : IQuery<GetTrackingStatisticsQueryResult>;
public record GetTrackingStatisticsQueryResult(IEnumerable<TrackingStatistic> TrackingStatistics);

public class GetTrackingStatisticsQueryHandler(ITrackingRepository trackingRepository) : IQueryHandler<GetTrackingStatisticsQuery, GetTrackingStatisticsQueryResult>
{
    public async Task<GetTrackingStatisticsQueryResult> HandleAsync(GetTrackingStatisticsQuery query, CancellationToken ct)
    {
        var statistics = await trackingRepository.GetStatistics(query.TrackingId, query.TimeRange, query.TimePeriod, ct);
        return new GetTrackingStatisticsQueryResult(statistics);
    }
}

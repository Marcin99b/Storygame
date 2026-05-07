using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Queries;

public record GetTrackingStatisticsQuery : IQuery<GetTrackingStatisticsQueryResult>;
public record GetTrackingStatisticsQueryResult;

public class GetTrackingStatisticsQueryHandler : IQueryHandler<GetTrackingStatisticsQuery, GetTrackingStatisticsQueryResult>
{
    public Task<GetTrackingStatisticsQueryResult> HandleAsync(GetTrackingStatisticsQuery query, CancellationToken ct)
    {

    }
}

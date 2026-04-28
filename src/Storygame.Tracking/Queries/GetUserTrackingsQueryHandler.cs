using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Queries;

public record GetUserTrackingsQuery(Guid UserId) : IQuery<GetUserTrackingsQueryResult>;

public record GetUserTrackingsQueryResult(IEnumerable<Tracking> Trackings);

public class GetUserTrackingsQueryHandler(ITrackingRepository trackingRepository) : IQueryHandler<GetUserTrackingsQuery, GetUserTrackingsQueryResult>
{
    public async Task<GetUserTrackingsQueryResult> HandleAsync(GetUserTrackingsQuery query, CancellationToken ct)
    {
        var trackings = await trackingRepository.GetUserTrackings(query.UserId, ct);
        return new GetUserTrackingsQueryResult(trackings);
    }
}

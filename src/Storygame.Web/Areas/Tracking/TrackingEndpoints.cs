using Microsoft.AspNetCore.Mvc;
using Storygame.Cqrs;
using Storygame.Tracking.Commands;
using Storygame.Tracking.Queries;

namespace Storygame.Web.Areas.Tracking;

public record StartTrackingRequest(Guid LibraryBookId, int TotalLength);
public record UpdateIndexRequest(int NewIndex);

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tracking").WithTags("Tracking").RequireAuthorization();

        group.MapGet("/", GetTrackings);
        group.MapPost("/", StartTracking);
        group.MapPost("/{libraryBookId:guid}/index", UpdateIndex);

        return app;
    }

    public static Task<GetUserTrackingsQueryResult> GetTrackings(IDispatcher dispatcher, UserSession userSession)
        => dispatcher.QueryAsync<GetUserTrackingsQuery, GetUserTrackingsQueryResult>(new GetUserTrackingsQuery(userSession.UserId!.Value));
    public static Task StartTracking(IDispatcher dispatcher, UserSession userSession, [FromBody] StartTrackingRequest request)
        => dispatcher.SendAsync(new StartTrackingBookCommand(request.LibraryBookId, userSession.UserId!.Value, request.TotalLength));

    public static Task UpdateIndex(IDispatcher dispatcher, UserSession userSession, [FromRoute] Guid trackingId, [FromBody] UpdateIndexRequest request)
        => dispatcher.SendAsync(new UpdateTrackingIndexCommand(trackingId, request.NewIndex));
}

using Microsoft.AspNetCore.Mvc;
using Storygame.Cqrs;
using Storygame.Tracking.Commands;
using Storygame.Tracking.Queries;

namespace Storygame.Web.Areas.Tracking;

public record StartTrackingRequest(int TotalLength);

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tracking").WithTags("Tracking").RequireAuthorization();

        group.MapGet("/", GetTrackings);
        group.MapPost("/{libraryBookId:guid}", StartTracking);

        return app;
    }

    public static Task<GetUserTrackingsQueryResult> GetTrackings(IDispatcher dispatcher, UserSession userSession)
        => dispatcher.QueryAsync<GetUserTrackingsQuery, GetUserTrackingsQueryResult>(new GetUserTrackingsQuery(userSession.UserId!.Value));
    public static Task StartTracking(IDispatcher dispatcher, UserSession userSession, [FromRoute] Guid libraryBookId, [FromBody] StartTrackingRequest request)
        => dispatcher.SendAsync(new StartTrackingBookCommand(libraryBookId, userSession.UserId!.Value, request.TotalLength));
}

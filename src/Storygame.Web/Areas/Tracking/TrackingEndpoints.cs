using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi;
using Storygame.Cqrs;
using Storygame.Library.Queries;
using Storygame.Ownership;
using Storygame.Tracking.Commands;
using Storygame.Tracking.Queries;
using Storygame.Web.Extencions;

namespace Storygame.Web.Areas.Tracking;

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tracking").WithTags("Tracking").RequireAuthorization();

        group.MapGet("/", GetTrackings);
        group.MapPost("/", StartTracking);
        group.MapPost("/{trackingId:guid}/index", UpdateIndex);

        return app;
    }

    public static async Task<GetTrackingsResponse> GetTrackings(IDispatcher dispatcher, UserSession userSession)
    {
        var result = await dispatcher.QueryAsync<GetUserTrackingsQuery, GetUserTrackingsQueryResult>(new GetUserTrackingsQuery(userSession.UserId!.Value));
        return result.ToResponse();
    }

    public static async Task StartTracking(IDispatcher dispatcher, UserSession userSession, [FromBody] StartTrackingRequest request)
    {
        var book = (await dispatcher.QueryAsync<GetLibraryBookByIdQuery, GetLibraryBookByIdQueryResult>(new GetLibraryBookByIdQuery(request.LibraryBookId))).Book;
        book.ThrowIfNotOwner(userSession.UserId!.Value);
        await dispatcher.SendAsync(new StartTrackingBookCommand(request.LibraryBookId, userSession.UserId!.Value, book.Length));
    }

    public static Task UpdateIndex(IDispatcher dispatcher, UserSession userSession, [FromRoute] Guid trackingId, [FromBody] UpdateIndexRequest request)
    {
        return dispatcher.SendAsync(new UpdateTrackingIndexCommand(trackingId, request.NewIndex));
    }
}

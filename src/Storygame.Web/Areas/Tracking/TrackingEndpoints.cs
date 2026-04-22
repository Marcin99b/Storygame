using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi;
using Storygame.Cqrs;
using Storygame.Library.Queries;
using Storygame.Ownership;
using Storygame.Tracking.Commands;
using Storygame.Tracking.Queries;
using Storygame.Web.Auth;
using Storygame.Web.Extencions;

namespace Storygame.Web.Areas.Tracking;

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tracking").WithTags("Tracking").RequireAuthorization(AuthExtensions.ActionIsRequestedByUserPolicy).RequireRateLimiting("MainRateLimiter");

        group.MapGet("/", GetTrackings);
        group.MapPost("/", StartTracking);
        group.MapPost("/{trackingId:guid}/index", UpdateIndex);

        return app;
    }

    public static async Task<GetTrackingsResponse> GetTrackings(IDispatcher dispatcher, UserSessionProvider sessionProvider, HttpContext context)
    {
        var session = sessionProvider.GetSession(context);
        var result = await dispatcher.QueryAsync<GetUserTrackingsQuery, GetUserTrackingsQueryResult>(new GetUserTrackingsQuery(session.UserId));
        return result.ToResponse();
    }

    public static async Task StartTracking(IDispatcher dispatcher, UserSessionProvider sessionProvider, HttpContext context, [FromBody] StartTrackingRequest request)
    {
        var session = sessionProvider.GetSession(context);
        var book = (await dispatcher.QueryAsync<GetLibraryBookByIdQuery, GetLibraryBookByIdQueryResult>(new GetLibraryBookByIdQuery(request.LibraryBookId, session.UserId))).Book;
        book.ThrowIfNotOwner(session.UserId);
        await dispatcher.SendAsync(new StartTrackingBookCommand(request.LibraryBookId, session.UserId, book.Length));
    }

    public static Task UpdateIndex(IDispatcher dispatcher, UserSessionProvider sessionProvider, HttpContext context, [FromRoute] Guid trackingId, [FromBody] UpdateIndexRequest request)
    {
        var session = sessionProvider.GetSession(context);
        return dispatcher.SendAsync(new UpdateTrackingIndexCommand(session.UserId, trackingId, request.NewIndex));
    }
}

using Microsoft.AspNetCore.Mvc;
using Storygame.Cqrs;
using Storygame.Tracking.Commands;

namespace Storygame.Web.Areas.Tracking;

public record StartTrackingRequest(int TotalLength);

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tracking").WithTags("Tracking").RequireAuthorization();

        group.MapGet("/", GetTrackings);
        group.MapPost("/{id:guid}", StartTracking);

        return app;
    }

    public static Task GetTrackings() => Task.CompletedTask;
    public static Task StartTracking(IDispatcher dispatcher, [FromRoute] Guid libraryBookId, [FromBody] StartTrackingRequest request) 
        => dispatcher.SendAsync(new StartTrackingBookCommand(libraryBookId, Guid.Empty, request.TotalLength));
}

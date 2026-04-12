namespace Storygame.Web.Areas.Tracking;

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tracking").WithTags("Tracking");

        group.MapGet("/", GetTrackings);
        group.MapGet("/{id:guid}", GetTrackingById);

        return app;
    }

    public static Task GetTrackings() => Task.CompletedTask;
    public static Task GetTrackingById() => Task.CompletedTask;
}

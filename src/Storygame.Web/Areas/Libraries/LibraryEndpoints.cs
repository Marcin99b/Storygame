using Microsoft.AspNetCore.Http.HttpResults;

namespace Storygame.Web.Areas.Library;

public static class LibraryEndpoints
{
    public static IEndpointRouteBuilder MapLibraryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/library").WithTags("Library");

        group.MapGet("/", GetLibrary);
        group.MapGet("/{id:guid}", GetLibraryBookById);

        return app;
    }

    public static Task GetLibrary() => Task.CompletedTask;
    public static Task GetLibraryBookById() => Task.CompletedTask;
}

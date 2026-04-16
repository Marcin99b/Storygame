using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storygame.Cqrs;
using Storygame.Library;
using Storygame.Library.Commands;
using Storygame.Library.Queries;

namespace Storygame.Web.Areas.Library;

public record AddToLibraryRequest(Guid? CatalogBookId, Guid? ImageId, string Title, string Description, MediaType MediaType, int Length);

public static class LibraryEndpoints
{
    public static IEndpointRouteBuilder MapLibraryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/library").WithTags("Library").RequireAuthorization();

        group.MapGet("/", GetLibrary);
        group.MapPost("/", AddToLibrary);

        return app;
    }

    public static Task GetLibrary(IDispatcher dispatcher) 
        => dispatcher.QueryAsync<GetUserBooksFromLibraryQuery, GetUserBooksFromLibraryQueryResult>(new GetUserBooksFromLibraryQuery(Guid.Empty));

    public static Task AddToLibrary(IDispatcher dispatcher, [FromBody] AddToLibraryRequest request)
        => dispatcher.SendAsync(new AddBookToLibraryCommand(Guid.Empty, request.CatalogBookId, request.ImageId, request.Title, request.Description, request.MediaType, request.Length));
}

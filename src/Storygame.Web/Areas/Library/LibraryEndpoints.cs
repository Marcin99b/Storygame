using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi;
using Storygame.Cqrs;
using Storygame.Library.Commands;
using Storygame.Library.Queries;
using Storygame.Web.Auth;
using Storygame.Web.Extencions;

namespace Storygame.Web.Areas.Library;

public static class LibraryEndpoints
{
    public static IEndpointRouteBuilder MapLibraryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/library").WithTags("Library").RequireAuthorization(AuthExtensions.ActionIsRequestedByUserPolicy);

        group.MapGet("/", GetLibrary);
        group.MapPost("/", AddToLibrary);

        return app;
    }

    public static async Task<GetLibraryResponse> GetLibrary(IDispatcher dispatcher, UserSession userSession)
    {
        var result = await dispatcher.QueryAsync<GetUserBooksFromLibraryQuery, GetUserBooksFromLibraryQueryResult>(new GetUserBooksFromLibraryQuery(userSession.UserId!.Value));
        return result.ToResponse();
    }

    public static Task AddToLibrary(IDispatcher dispatcher, UserSession userSession, [FromBody] AddToLibraryRequest request)
    {
        return dispatcher.SendAsync(new AddBookToLibraryCommand(userSession.UserId!.Value, request.CatalogBookId, request.ImageId, request.Title, request.Description, (Storygame.Library.MediaType)request.MediaType, request.Length));
    }
}

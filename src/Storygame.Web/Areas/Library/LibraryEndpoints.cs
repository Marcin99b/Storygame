using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi.Requests;
using Storygame.Contracts.WebApi.Responses;
using Storygame.Cqrs;
using Storygame.Library.Commands;
using Storygame.Library.Queries;
using Storygame.Web.Auth;
using Storygame.Web.Extensions;

namespace Storygame.Web.Areas.Library;

public static class LibraryEndpoints
{
    public static IEndpointRouteBuilder MapLibraryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/library").WithTags("Library")
            .RequireAuthorization(AuthExtensions.ActionIsRequestedByUserPolicy)
            .RequireRateLimiting("MainRateLimiter")
            .ValidateAntiforgery();

        group.MapGet("/", GetLibrary);
        group.MapPost("/", AddToLibrary);

        return app;
    }

    public static async Task<GetLibraryResponse> GetLibrary(IDispatcher dispatcher, UserSessionProvider sessionProvider, HttpContext context)
    {
        var session = sessionProvider.GetSession(context);
        var result = await dispatcher.QueryAsync<GetUserBooksFromLibraryQuery, GetUserBooksFromLibraryQueryResult>(new GetUserBooksFromLibraryQuery(session.UserId));
        return result.ToResponse();
    }

    public static Task AddToLibrary(IDispatcher dispatcher, UserSessionProvider sessionProvider, HttpContext context, [FromBody] AddToLibraryRequest request)
    {
        var session = sessionProvider.GetSession(context);
        return dispatcher.SendAsync(new AddBookToLibraryCommand(session.UserId, request.CatalogBookId, request.ImageId, request.Title, request.Description, (Storygame.Library.MediaType)request.MediaType, request.Length));
    }
}

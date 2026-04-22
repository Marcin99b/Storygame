using Microsoft.AspNetCore.Mvc;
using Storygame.Catalog.Queries;
using Storygame.Contracts.WebApi;
using Storygame.Cqrs;
using Storygame.Web.Auth;
using Storygame.Web.Extencions;

namespace Storygame.Web.Areas.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog").RequireAuthorization(AuthExtensions.ActionIsRequestedByUserPolicy);

        group.MapGet("/", GetCatalog);

        return app;
    }

    public static async Task<GetCatalogResponse> GetCatalog(IDispatcher dispatcher, [FromQuery] string? titleContains, [FromQuery] bool? hasTextEdition, [FromQuery] bool? hasAudiobook)
    {
        var result = await dispatcher.QueryAsync<SearchCatalogQuery, SearchCatalogQueryResult>(new SearchCatalogQuery(titleContains, hasTextEdition, hasAudiobook));
        return result.ToResponse();
    }
}


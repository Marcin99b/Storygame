using Microsoft.AspNetCore.Mvc;
using Storygame.Catalog.Queries;
using Storygame.Cqrs;

namespace Storygame.Web.Areas.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog");

        group.MapGet("/", GetCatalog);

        return app;
    }

    public static Task<SearchCatalogQueryResult> GetCatalog(IDispatcher dispatcher, [FromQuery] string? titleContains, [FromQuery] bool? hasTextEdition, [FromQuery] bool? hasAudiobook)
        => dispatcher.QueryAsync<SearchCatalogQuery, SearchCatalogQueryResult>(new SearchCatalogQuery(titleContains, hasTextEdition, hasAudiobook));
}

using Storygame.Catalog.Queries;
using Storygame.Cqrs;

namespace Storygame.Web.Areas.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog");

        group.MapGet("/", GetCatalog);
        group.MapGet("/{id:guid}", GetCatalogBookById);

        return app;
    }

    public static Task<SearchCatalogQueryResult> GetCatalog(IDispatcher dispatcher)
        => dispatcher.QueryAsync<SearchCatalogQuery, SearchCatalogQueryResult>(new SearchCatalogQuery());

    public static Task GetCatalogBookById() => Task.CompletedTask;
}

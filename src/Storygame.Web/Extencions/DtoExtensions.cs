using Storygame.Catalog.Queries;
using Storygame.Contracts.WebApi;

namespace Storygame.Web.Extencions;

public static class DtoExtensions
{
    extension(SearchCatalogQueryResult result)
    {
        public GetCatalogResponse ToResponse()
        {
            var books = result.Books.Select(x => new CatalogBookDto(x.Id, x.ImageId, x.Title, x.Description,
                new TextEditionFieldsDto(x.TextEditionFields.Exist, x.TextEditionFields.TotalPages),
                new AudiobookFieldsDto(x.AudiobookFields.Exist, x.AudiobookFields.TotalMinutes)))
                .ToArray();
            return new GetCatalogResponse(books);
        }
    }
}

using Storygame.Contracts.WebApi.Dtos;

namespace Storygame.Contracts.WebApi.Responses;

public record GetCatalogResponse(CatalogBookDto[] Books);

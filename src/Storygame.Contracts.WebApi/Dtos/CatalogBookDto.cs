namespace Storygame.Contracts.WebApi.Dtos;

public record CatalogBookDto(Guid Id, Guid? ImageId, string Title, string Description, TextEditionFieldsDto TextEditionFields, AudiobookFieldsDto AudiobookFields);

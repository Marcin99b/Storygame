namespace Storygame.Contracts.WebApi;

public record CatalogBookDto(Guid Id, Guid? ImageId, string Title, string Description, TextEditionFieldsDto TextEditionFields, AudiobookFieldsDto AudiobookFields);

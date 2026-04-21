namespace Storygame.Contracts.WebApi;

public record LibraryBookDto(Guid Id, Guid UserId, Guid? CatalogBookId, Guid? ImageId, string Title, string Description, MediaTypeDto MediaType, int Length, DateTime AddedToLibraryAt, LengthUnitDto LengthUnit);

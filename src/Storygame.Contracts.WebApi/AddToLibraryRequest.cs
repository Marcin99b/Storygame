namespace Storygame.Contracts.WebApi;

public record AddToLibraryRequest(Guid? CatalogBookId, Guid? ImageId, string Title, string Description, MediaTypeDto MediaType, int Length);
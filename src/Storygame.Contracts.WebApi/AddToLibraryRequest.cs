namespace Storygame.Contracts.WebApi;

public record AddToLibraryRequest(Guid? CatalogBookId, Guid? ImageId, string Title, string Description, MediaType MediaType, int Length);

public enum MediaType
{
    Ebook,
    Paperback,
    Audiobook
}
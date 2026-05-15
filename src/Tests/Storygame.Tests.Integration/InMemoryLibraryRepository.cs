using Storygame.Library;
using System.Collections.Concurrent;

namespace Storygame.Tests.Integration;

internal class InMemoryLibraryRepository : ILibraryRepository
{
    private readonly ConcurrentDictionary<Guid, Book> _books = new();

    public Task AddBook(Book book, CancellationToken ct)
    {
        _books[book.Id] = book;
        return Task.CompletedTask;
    }

    public Task<bool> CheckIfUserAlreadyHasThisBook(Guid userId, Guid catalogBookId, MediaType mediaType, CancellationToken ct)
    {
        var exists = _books.Values.Any(b => b.UserId == userId && b.CatalogBookId == catalogBookId && b.MediaType == mediaType);
        return Task.FromResult(exists);
    }

    public Task<IEnumerable<Book>> GetUserBooks(Guid userId, CancellationToken ct)
        => Task.FromResult(_books.Values.Where(b => b.UserId == userId));

    public Task<Book> GetBookById(Guid bookId, Guid userId, CancellationToken ct)
        => Task.FromResult(_books.Values.Single(b => b.Id == bookId && b.UserId == userId));
}

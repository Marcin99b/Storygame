using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library;

public interface ILibraryRepository
{
    public Task AddBook(Book book, CancellationToken ct);
    public Task<bool> CheckIfUserAlreadyHasThisBook(Guid userId, Guid catalogBookId, MediaType mediaType, CancellationToken ct);
    public Task<IEnumerable<Book>> GetUserBooks(Guid userId, CancellationToken ct);
    public Task<Book> GetBookById(Guid bookId, Guid userId, CancellationToken ct);
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library;

public interface ILibraryRepository
{
    public Task AddBook(Book book);
    public Task<bool> CheckIfUserAlreadyHasThisBook(Guid userId, Guid catalogBookId, MediaType mediaType);
    public Task<IEnumerable<Book>> GetUserBooks(Guid userId);
    public Task<Book> GetBookById(Guid bookId, Guid userId);
}

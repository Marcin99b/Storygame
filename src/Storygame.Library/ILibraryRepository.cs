using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library;

public interface ILibraryRepository
{
    public void AddBook(Book book);
    public Task<bool> CheckIfUserAlreadyHasThisBook(Guid catalogBookId, MediaType mediaType);
    public Task<IEnumerable<Book>> GetUserBooks(Guid userId);
}

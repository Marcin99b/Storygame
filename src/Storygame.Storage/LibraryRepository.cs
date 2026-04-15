using Storygame.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Storage;

public class LibraryRepository : ILibraryRepository
{
    public void AddBook(Book book)
    {
    }

    public Task<bool> CheckIfUserAlreadyHasThisBook(Guid catalogBookId, MediaType mediaType) => throw new NotImplementedException();
    public Task<IEnumerable<Book>> GetUserBooks(Guid userId) => throw new NotImplementedException();
}

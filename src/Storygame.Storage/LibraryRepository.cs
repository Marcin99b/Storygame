using MongoDB.Driver;
using Storygame.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Storage;

public class LibraryRepository(IMongoDatabase database) : ILibraryRepository
{
    private readonly IMongoCollection<Book> books = database.GetCollection<Book>(DbCollectionNames.LIBRARY_BOOKS);

    public void AddBook(Book book)
    {
    }

    public Task<bool> CheckIfUserAlreadyHasThisBook(Guid catalogBookId, MediaType mediaType) => throw new NotImplementedException();
    public Task<IEnumerable<Book>> GetUserBooks(Guid userId) => throw new NotImplementedException();
}

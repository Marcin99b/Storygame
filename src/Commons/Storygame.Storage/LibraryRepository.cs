using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Storage;

//todo use cancellation tokens
public class LibraryRepository(IMongoDatabase database) : ILibraryRepository
{
    private readonly IMongoCollection<Book> books = database.GetCollection<Book>(DbCollectionNames.LIBRARY_BOOKS);

    public async Task AddBook(Book book, CancellationToken ct)
    {
        await books.InsertOneAsync(book, null, ct);
    }

    public async Task<bool> CheckIfUserAlreadyHasThisBook(Guid userId, Guid catalogBookId, MediaType mediaType, CancellationToken ct)
        => await books.CountDocumentsAsync(x => x.UserId == userId && x.CatalogBookId == catalogBookId && x.MediaType == mediaType, null, ct) > 0;

    public async Task<Book> GetBookById(Guid bookId, Guid userId, CancellationToken ct) 
        => await books.AsQueryable().FirstAsync(x => x.Id == bookId && x.UserId == userId, ct);

    public async Task<IEnumerable<Book>> GetUserBooks(Guid userId, CancellationToken ct) 
        => await books.AsQueryable().Where(x => x.UserId == userId).ToListAsync(ct);
}

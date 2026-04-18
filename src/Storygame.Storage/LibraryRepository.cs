using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Storage;

//todo async Add() and use cancellation tokens
public class LibraryRepository(IMongoDatabase database) : ILibraryRepository
{
    private readonly IMongoCollection<Book> books = database.GetCollection<Book>(DbCollectionNames.LIBRARY_BOOKS);

    public void AddBook(Book book)
    {
        books.InsertOne(book);
    }

    public Task<bool> CheckIfUserAlreadyHasThisBook(Guid catalogBookId, MediaType mediaType) => throw new NotImplementedException();

    public async Task<IEnumerable<Book>> GetUserBooks(Guid userId) 
        => await books.AsQueryable().Where(x => x.UserId == userId).ToListAsync();
}

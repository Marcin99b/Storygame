using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Events;

public record BookAddedToLibraryEvent(Guid LibraryBookId, Guid UserId, Guid? CatalogBookId, Guid? ImageId, string Title, string Description, int Length, MediaType MediaType, DateTime AddedToLibraryAt) : Event
{
    public Guid EventId { get; } = Guid.NewGuid();

    public static BookAddedToLibraryEvent FromBook(Book book)
        => new (book.Id, book.UserId, book.CatalogBookId, book.ImageId, book.Title, book.Description, book.Length, book.MediaType, book.AddedToLibraryAt);
}
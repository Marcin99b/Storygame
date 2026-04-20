using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Commands;

public record AddBookToLibraryCommand(Guid UserId, Guid? CatalogBookId, Guid? ImageId, string Title, string Description, MediaType MediaType, int Length) : ICommand;

public class AddBookToLibraryCommandHandler(ILibraryRepository libraryRepository) : ICommandHandler<AddBookToLibraryCommand>
{
    public async Task HandleAsync(AddBookToLibraryCommand command)
    {
        var alreadyExist = command.CatalogBookId.HasValue 
            && await libraryRepository.CheckIfUserAlreadyHasThisBook(command.UserId, command.CatalogBookId.Value, command.MediaType);
        if (alreadyExist)
        {
            throw new ArgumentException($"User already has book: {command.CatalogBookId!.Value} with media type: {command.MediaType.ToString()}");
        }

        var book = new Book()
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CatalogBookId = command.CatalogBookId,
            ImageId = command.ImageId,
            Title = command.Title,
            Description = command.Description,
            Length = command.Length,
            MediaType = command.MediaType,
            AddedToLibraryAt = DateTime.UtcNow
        };

        libraryRepository.AddBook(book);
    }
}

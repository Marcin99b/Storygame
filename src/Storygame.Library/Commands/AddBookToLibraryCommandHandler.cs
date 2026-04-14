using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Commands;

public record AddBookToLibraryCommand() : ICommand;

public class AddBookToLibraryCommandHandler : ICommandHandler<AddBookToLibraryCommand>
{
    public Task HandleAsync(AddBookToLibraryCommand command) => throw new NotImplementedException();
}

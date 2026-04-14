using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Commands;

public record StartTrackingBookCommand : ICommand;

public class StartTrackingBookCommandHandler : ICommandHandler<StartTrackingBookCommand>
{
    public Task HandleAsync(StartTrackingBookCommand command) => throw new NotImplementedException();
}

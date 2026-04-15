using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Commands;

public record StartTrackingBookCommand(Guid LibraryBookId, Guid UserId, int TotalLength) : ICommand;

public class StartTrackingBookCommandHandler(ITrackingRepository trackingRepository) : ICommandHandler<StartTrackingBookCommand>
{
    public async Task HandleAsync(StartTrackingBookCommand command)
    {
        if (await trackingRepository.CheckIfBookIsAlreadyTracked(command.LibraryBookId))
        {
            throw new ArgumentException($"Book {command.LibraryBookId} is already tracked");
        }

        var tracking = new Tracking()
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CurrentIndex = 0,
            LibraryBookId = command.LibraryBookId,
            TotalLength = command.TotalLength
        };

        trackingRepository.AddTracking(tracking);
    }
}

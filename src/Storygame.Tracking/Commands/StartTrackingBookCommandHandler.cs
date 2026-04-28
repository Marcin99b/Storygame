using Storygame.Cqrs;
using Storygame.Tracking.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Commands;

public record StartTrackingBookCommand(Guid LibraryBookId, Guid UserId, int TotalLength) : ICommand;

public class StartTrackingBookCommandHandler(ITrackingRepository trackingRepository, IDispatcher dispatcher) : ICommandHandler<StartTrackingBookCommand>
{
    public async Task HandleAsync(StartTrackingBookCommand command, CancellationToken ct)
    {
        if (await trackingRepository.CheckIfBookIsAlreadyTracked(command.LibraryBookId, ct))
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

        await trackingRepository.AddTracking(tracking, ct);

        await dispatcher.PublishAsync(TrackingStartedEvent.FromTracking(tracking), ct);
    }
}

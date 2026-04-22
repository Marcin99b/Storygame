using Storygame.Cqrs;
using Storygame.Ownership;
using Storygame.Tracking.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Commands;

public record UpdateTrackingIndexCommand(Guid TrackingId, int NewIndex) : ICommand;

public class UpdateTrackingIndexCommandHandler(ITrackingRepository trackingRepository, IDispatcher dispatcher) : ICommandHandler<UpdateTrackingIndexCommand>
{
    public async Task HandleAsync(UpdateTrackingIndexCommand command)
    {
        var tracking = await trackingRepository.GetTracking(command.TrackingId);
        tracking.ThrowIfNotOwner(command.TrackingId);

        var oldIndex = tracking.CurrentIndex;
        tracking.CurrentIndex = command.NewIndex;
        await trackingRepository.UpdateTracking(tracking);
        await dispatcher.PublishAsync(new TrackingIndexUpdatedEvent(tracking.Id, oldIndex, tracking.CurrentIndex));
    }
}

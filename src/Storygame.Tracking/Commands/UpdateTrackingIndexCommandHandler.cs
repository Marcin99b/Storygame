using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Commands;

public record UpdateTrackingIndexCommand(Guid TrackingId, int NewIndex) : ICommand;

public class UpdateTrackingIndexCommandHandler(ITrackingRepository trackingRepository) : ICommandHandler<UpdateTrackingIndexCommand>
{
    public async Task HandleAsync(UpdateTrackingIndexCommand command)
    {
        var tracking = await trackingRepository.GetTracking(command.TrackingId);
        tracking.CurrentIndex = command.NewIndex;
        await trackingRepository.UpdateTracking(tracking);
    }
}

using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Commands;

public record UpdateTrackingIndexCommand : ICommand;

public class UpdateTrackingIndexCommandHandler(ITrackingRepository trackingRepository) : ICommandHandler<UpdateTrackingIndexCommand>
{
    public Task HandleAsync(UpdateTrackingIndexCommand command)
    {

    }
}

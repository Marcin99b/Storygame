using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Events;

public record TrackingIndexUpdatedEvent(Guid TrackingId, int NewIndex) : IEvent
{
    public static TrackingIndexUpdatedEvent FromTracking(Tracking tracking)
        => new (tracking.Id, tracking.CurrentIndex);
}
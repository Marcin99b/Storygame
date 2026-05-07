using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking.Events;

public record TrackingIndexUpdatedEvent(Guid TrackingId, int NewIndex, int OldIndex) : Event
{
    public static TrackingIndexUpdatedEvent FromTracking(Tracking tracking, int oldIndex)
        => new (tracking.Id, tracking.CurrentIndex, oldIndex);
}
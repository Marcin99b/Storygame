using Storygame.Cqrs;

namespace Storygame.Tracking.Events;

public record TrackingStartedEvent(Guid TrackingId, Guid LibraryBookId, Guid UserId, int TotalLength) : IEvent
{
    public static TrackingStartedEvent FromTracking(Tracking tracking)
        => new (tracking.Id, tracking.LibraryBookId, tracking.UserId, tracking.TotalLength);
}
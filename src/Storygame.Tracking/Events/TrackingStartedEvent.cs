using Storygame.Cqrs;

namespace Storygame.Tracking.Events;

public record TrackingStartedEvent(Guid TrackingId, Guid LibraryBookId, Guid UserId, int TotalLength) : IEvent;
using Storygame.Cqrs;

namespace Storygame.Users.Events;

public record UserVerifiedEvent(Guid UserId, DateTime VerifiedAt) : IEvent;
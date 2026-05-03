using Storygame.Cqrs;

namespace Storygame.Users.Events;

public record UserVerifiedEvent(Guid UserId, DateTime VerifiedAt) : Event
{
    public Guid EventId { get; } = Guid.NewGuid();

    public static UserVerifiedEvent FromUser(User user)
        => new (user.Id, user.VerifiedAt!.Value);
}
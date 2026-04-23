using Storygame.Cqrs;

namespace Storygame.Users.Events;

public record UserVerifiedEvent(Guid UserId, DateTime VerifiedAt) : IEvent
{
    public static UserVerifiedEvent FromUser(User user)
        => new (user.Id, user.VerifiedAt!.Value);
}
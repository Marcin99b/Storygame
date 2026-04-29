using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Users.Events;

public record UserRegisteredEvent(Guid UserId, string Name, string Email, DateTime RegisteredAt) : IEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public static UserRegisteredEvent FromUser(User user)
        => new (user.Id, user.Name, user.Email, user.RegisteredAt);
}

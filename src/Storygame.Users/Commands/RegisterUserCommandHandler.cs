using Storygame.Cqrs;
using Storygame.Users.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Users.Commands;

public record RegisterUserCommand(string Name, string Email) : ICommand;

public class RegisterUserCommandHandler(IUsersRepository usersRepository, IDispatcher dispatcher) : ICommandHandler<RegisterUserCommand>
{
    public async Task HandleAsync(RegisterUserCommand command)
    {
        if (await usersRepository.CheckIfEmailExist(command.Email))
        {
            throw new ArgumentException($"User with email {command.Email} is already registered");
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            RegisteredAt = DateTime.UtcNow,
            VerifiedAt = null,
        };

        await usersRepository.AddUser(user);
        await dispatcher.PublishAsync(new UserRegisteredEvent(user.Id, user.Name, user.Email, user.RegisteredAt));
        //todo event handler should send email with verification link
    }
}

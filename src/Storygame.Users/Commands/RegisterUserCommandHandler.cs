using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Users.Commands;

public record RegisterUserCommand(string Name, string Email) : ICommand;

internal class RegisterUserCommandHandler(IUsersRepository usersRepository) : ICommandHandler<RegisterUserCommand>
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
    }
}

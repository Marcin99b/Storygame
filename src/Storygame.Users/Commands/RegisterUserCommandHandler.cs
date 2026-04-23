using Storygame.Cqrs;
using Storygame.Integrations.Email;
using Storygame.Users.Events;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Storygame.Users.Commands;

public record RegisterUserCommand(string Name, string Email) : ICommand;

public class RegisterUserCommandHandler(IUsersRepository usersRepository, EmailClient emailClient, IDispatcher dispatcher) : ICommandHandler<RegisterUserCommand>
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

        var code = RandomNumberGenerator.GetHexString(6).ToUpper();
        var verificationCode = new UserVerificationCode(Guid.NewGuid(), user.Id, code);
        await usersRepository.SaveUserVerificationCode(verificationCode);
        await emailClient.Send(new MailMessage(user.Email, "Verification code", code, DateTime.UtcNow));

        await dispatcher.PublishAsync(UserRegisteredEvent.FromUser(user));
    }
}

using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Users.Commands;

public record VerifyUserCommand(string Email, string VerificationCode) : ICommand;

public class VerifyUserCommandHandler(IUsersRepository usersRepository) : ICommandHandler<VerifyUserCommand>
{
    public async Task HandleAsync(VerifyUserCommand command)
    {
        var user = await usersRepository.GetUserByEmail(command.Email);
        if (user.IsVerified)
        {
            throw new ArgumentException($"User with email {command.Email} is already verified");
        }

        var verificationCode = await usersRepository.GetUserVerificationCode(user.Id);

        if (command.VerificationCode != verificationCode.Code)
        {
            throw new ArgumentException($"Verification code for user {user.Id} is wrong");
        }

        user.VerifiedAt = DateTime.UtcNow;
        await usersRepository.UpdateUser(user);
    }
}

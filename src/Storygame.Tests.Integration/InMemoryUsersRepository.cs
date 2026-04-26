using Storygame.Users;
using System.Collections.Concurrent;

namespace Storygame.Tests.Integration;

internal class InMemoryUsersRepository : IUsersRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly ConcurrentDictionary<Guid, UserVerificationCode> _verificationCodes = new();

    public Task AddUser(User user)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateUser(User user)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task<User> GetUserById(Guid userId)
        => Task.FromResult(_users[userId]);

    public Task<User> GetUserByEmail(string email)
        => Task.FromResult(_users.Values.Single(u => u.Email == email));

    public Task<bool> CheckIfEmailExist(string email)
        => Task.FromResult(_users.Values.Any(u => u.Email == email));

    public Task SaveUserVerificationCode(UserVerificationCode verificationCode)
    {
        _verificationCodes[verificationCode.UserId] = verificationCode;
        return Task.CompletedTask;
    }

    public Task<UserVerificationCode> GetUserVerificationCode(Guid userId)
        => Task.FromResult(_verificationCodes[userId]);
}

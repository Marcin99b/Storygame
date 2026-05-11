using Storygame.Users;
using System.Collections.Concurrent;

namespace Storygame.Tests.Integration;

internal class InMemoryUsersRepository : IUsersRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly ConcurrentDictionary<Guid, UserVerificationCode> _verificationCodes = new();

    public Task AddUser(User user, CancellationToken ct)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateUser(User user, CancellationToken ct)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task<User> GetUserById(Guid userId, CancellationToken ct)
        => Task.FromResult(_users[userId]);

    public Task<User> GetUserByEmail(string email, CancellationToken ct)
        => Task.FromResult(_users.Values.Single(u => u.Email == email));

    public Task<bool> CheckIfEmailExist(string email, CancellationToken ct)
        => Task.FromResult(_users.Values.Any(u => u.Email == email));

    public Task SaveUserVerificationCode(UserVerificationCode verificationCode, CancellationToken ct)
    {
        _verificationCodes[verificationCode.UserId] = verificationCode;
        return Task.CompletedTask;
    }

    public Task<UserVerificationCode> GetUserVerificationCode(Guid userId, CancellationToken ct)
        => Task.FromResult(_verificationCodes[userId]);
}

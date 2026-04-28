using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Storage;

public class UsersRepository(IMongoDatabase database) : IUsersRepository
{
    private readonly IMongoCollection<User> users = database.GetCollection<User>(DbCollectionNames.USERS);
    private readonly IMongoCollection<UserVerificationCode> verificationCodes = database.GetCollection<UserVerificationCode>(DbCollectionNames.USERS_VERIFICATION_CODES);

    public Task AddUser(User user, CancellationToken ct)
    {
        return users.InsertOneAsync(user, null, ct);
    }

    public Task UpdateUser(User user, CancellationToken ct)
    {
        return users.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken: ct);
    }

    public async Task<bool> CheckIfEmailExist(string email, CancellationToken ct)
    {
        return (await users.CountDocumentsAsync(x => x.Email == email, null, ct)) > 0;
    }

    public async Task<User> GetUserById(Guid userId, CancellationToken ct)
    {
        return await users.AsQueryable().FirstAsync(x => x.Id == userId, ct);
    }

    public async Task<User> GetUserByEmail(string email, CancellationToken ct)
    {
        return await users.AsQueryable().FirstAsync(x => x.Email == email, ct);
    }

    public async Task SaveUserVerificationCode(UserVerificationCode verificationCode, CancellationToken ct)
    {
        await verificationCodes.InsertOneAsync(verificationCode, null, ct);
    }

    public async Task<UserVerificationCode> GetUserVerificationCode(Guid userId, CancellationToken ct)
    {
        return await verificationCodes.AsQueryable().FirstAsync(x => x.UserId == userId, ct);
    }
}

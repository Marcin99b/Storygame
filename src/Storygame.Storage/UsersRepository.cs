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

    public Task AddUser(User user)
    {
        return users.InsertOneAsync(user);
    }

    public Task UpdateUser(User user)
    {
        return users.ReplaceOneAsync(x => x.Id == user.Id, user);
    }

    public async Task<bool> CheckIfEmailExist(string email)
    {
        return (await users.CountDocumentsAsync(x => x.Email == email)) > 0;
    }

    public async Task<User> GetUserById(Guid userId)
    {
        return await users.AsQueryable().FirstAsync(x => x.Id == userId);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await users.AsQueryable().FirstAsync(x => x.Email == email);
    }

    public async Task SaveUserVerificationCode(UserVerificationCode verificationCode)
    {
        await verificationCodes.InsertOneAsync(verificationCode);
    }

    public async Task<UserVerificationCode> GetUserVerificationCode(Guid userId)
    {
        return await verificationCodes.AsQueryable().FirstAsync(x => x.UserId == userId);
    }
}

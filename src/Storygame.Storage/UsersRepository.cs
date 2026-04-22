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

    public Task AddUser(User user)
    {
        return users.InsertOneAsync(user);
    }

    public async Task<bool> CheckIfEmailExist(string email)
    {
        return (await users.CountDocumentsAsync(x => x.Email == email)) > 0;
    }

    public Task<User> GetUserById(Guid userId)
    {
        return users.AsQueryable().FirstOrDefaultAsync(x => x.Id == userId);
    }
}

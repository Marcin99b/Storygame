namespace Storygame.Users;

public interface IUsersRepository
{
    Task AddUser(User user);
    Task<User> GetUserById(Guid userId);
    Task<User> GetUserByEmail(string email);
    Task<bool> CheckIfEmailExist(string email);
}
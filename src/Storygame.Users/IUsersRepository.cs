namespace Storygame.Users;

public interface IUsersRepository
{
    Task AddUser(User user);
    Task UpdateUser(User user);
    Task<User> GetUserById(Guid userId);
    Task<User> GetUserByEmail(string email);
    Task<bool> CheckIfEmailExist(string email);
    Task SaveUserVerificationCode(UserVerificationCode verificationCode);
    Task<UserVerificationCode> GetUserVerificationCode(Guid userId);
}
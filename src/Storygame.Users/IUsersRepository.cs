namespace Storygame.Users;

public interface IUsersRepository
{
    Task AddUser(User user, CancellationToken ct);
    Task UpdateUser(User user, CancellationToken ct);
    Task<User> GetUserById(Guid userId, CancellationToken ct);
    Task<User> GetUserByEmail(string email, CancellationToken ct);
    Task<bool> CheckIfEmailExist(string email, CancellationToken ct);
    Task SaveUserVerificationCode(UserVerificationCode verificationCode, CancellationToken ct);
    Task<UserVerificationCode> GetUserVerificationCode(Guid userId, CancellationToken ct);
}
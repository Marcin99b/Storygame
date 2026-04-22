namespace Storygame.Users;

public record UserVerificationCode(Guid Id, Guid UserId, string Code);
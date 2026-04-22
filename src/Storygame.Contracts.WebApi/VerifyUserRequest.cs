namespace Storygame.Contracts.WebApi;

public record VerifyUserRequest(string Email, string VerificationCode);

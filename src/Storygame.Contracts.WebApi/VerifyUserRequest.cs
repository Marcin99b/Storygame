using System.ComponentModel.DataAnnotations;

namespace Storygame.Contracts.WebApi;

public record VerifyUserRequest([MaxLength(100)][EmailAddress] string Email, [MaxLength(100)] string VerificationCode);

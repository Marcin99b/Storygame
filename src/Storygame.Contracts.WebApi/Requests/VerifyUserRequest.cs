using System.ComponentModel.DataAnnotations;

namespace Storygame.Contracts.WebApi.Requests;

public record VerifyUserRequest([MaxLength(100)][EmailAddress] string Email, [MaxLength(100)] string VerificationCode);

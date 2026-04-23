using System.ComponentModel.DataAnnotations;

namespace Storygame.Contracts.WebApi;

public record LoginRequest([MaxLength(100)] [EmailAddress] string Email);

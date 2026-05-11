using System.ComponentModel.DataAnnotations;

namespace Storygame.Contracts.WebApi;

public record RegisterRequest([MaxLength(30)] string Name, [MaxLength(100)][EmailAddress] string Email);

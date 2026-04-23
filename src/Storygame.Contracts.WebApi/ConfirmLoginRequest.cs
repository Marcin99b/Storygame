using System.ComponentModel.DataAnnotations;

namespace Storygame.Contracts.WebApi;

public record ConfirmLoginRequest([MaxLength(100)]string LoginConfirmationKey);
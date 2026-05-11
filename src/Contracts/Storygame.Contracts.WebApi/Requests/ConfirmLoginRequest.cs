using System.ComponentModel.DataAnnotations;

namespace Storygame.Contracts.WebApi.Requests;

public record ConfirmLoginRequest([MaxLength(100)]string LoginConfirmationKey);
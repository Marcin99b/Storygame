using Storygame.Contracts.WebApi.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Storygame.Contracts.WebApi.Requests;

public record AddToLibraryRequest(Guid? CatalogBookId, Guid? ImageId, [MaxLength(30)] string Title, [MaxLength(500)] string Description, MediaTypeDto MediaType, int Length);
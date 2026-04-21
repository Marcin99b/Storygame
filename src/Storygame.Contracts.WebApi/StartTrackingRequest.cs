namespace Storygame.Contracts.WebApi;

public record StartTrackingRequest(Guid LibraryBookId, int TotalLength);

using Storygame.Contracts.WebApi.Dtos;

namespace Storygame.Contracts.WebApi.Responses;

public record GetTrackingsResponse(TrackingDto[] Trackings);
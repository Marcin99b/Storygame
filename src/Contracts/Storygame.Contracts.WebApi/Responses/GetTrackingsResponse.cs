using Storygame.Contracts.WebApi.Dtos;

namespace Storygame.Contracts.WebApi.Responses;

public record GetTrackingsResponse(TrackingDto[] Trackings);

public record GetStatisticsResponse(TrackingStatisticDto[] TrackingStatistics);

public record TrackingStatisticDto(TimePeriodDto TimePeriod, DateTime From, DateTime To, int Value);

public enum TimePeriodDto
{
    Day,
    Week,
    Month,
    Year
}
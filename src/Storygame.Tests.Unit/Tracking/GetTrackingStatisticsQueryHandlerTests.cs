using Moq;
using Shouldly;
using Storygame.Tracking;
using Storygame.Tracking.Queries;

namespace Storygame.Tests.Unit.Tracking;

[TestFixture]
public class GetTrackingStatisticsQueryHandlerTests
{
    private Mock<ITrackingRepository> _repository = null!;
    private GetTrackingStatisticsQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ITrackingRepository>();
        _handler = new GetTrackingStatisticsQueryHandler(_repository.Object);
    }

    [Test]
    public async Task Returns_statistics_for_given_tracking_and_time_range()
    {
        var trackingId = Guid.NewGuid();
        var timeRange = new TimeRange(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var statistics = new List<TrackingStatistic>
        {
            new() { Id = Guid.NewGuid(), TrackingId = trackingId, TimePeriod = TimePeriod.Day, TimeRange = timeRange, Value = 30 },
            new() { Id = Guid.NewGuid(), TrackingId = trackingId, TimePeriod = TimePeriod.Day, TimeRange = timeRange, Value = 45 }
        };

        _repository.Setup(r => r.GetStatistics(trackingId, timeRange, TimePeriod.Day, It.IsAny<CancellationToken>()))
            .ReturnsAsync(statistics);

        var result = await _handler.HandleAsync(
            new GetTrackingStatisticsQuery(trackingId, timeRange, TimePeriod.Day), CancellationToken.None);

        result.TrackingStatistics.ShouldBe(statistics);
    }

    [Test]
    public async Task Returns_empty_when_no_statistics_exist_for_given_period()
    {
        var trackingId = Guid.NewGuid();
        var timeRange = new TimeRange(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

        _repository.Setup(r => r.GetStatistics(trackingId, timeRange, TimePeriod.Month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<TrackingStatistic>());

        var result = await _handler.HandleAsync(
            new GetTrackingStatisticsQuery(trackingId, timeRange, TimePeriod.Month), CancellationToken.None);

        result.TrackingStatistics.ShouldBeEmpty();
    }

    [Test]
    public async Task Passes_all_query_parameters_to_repository()
    {
        var trackingId = Guid.NewGuid();
        var timeRange = new TimeRange(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

        _repository.Setup(r => r.GetStatistics(It.IsAny<Guid>(), It.IsAny<TimeRange>(), It.IsAny<TimePeriod>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<TrackingStatistic>());

        await _handler.HandleAsync(new GetTrackingStatisticsQuery(trackingId, timeRange, TimePeriod.Week), CancellationToken.None);

        _repository.Verify(r => r.GetStatistics(trackingId, timeRange, TimePeriod.Week, It.IsAny<CancellationToken>()), Times.Once);
    }
}

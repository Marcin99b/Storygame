using Moq;
using Shouldly;
using Storygame.Tracking;
using Storygame.Tracking.Queries;
using TrackingEntry = Storygame.Tracking.Tracking;

namespace Storygame.Tests.Unit.Tracking;

[TestFixture]
public class GetUserTrackingsQueryHandlerTests
{
    private Mock<ITrackingRepository> _repository = null!;
    private GetUserTrackingsQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ITrackingRepository>();
        _handler = new GetUserTrackingsQueryHandler(_repository.Object);
    }

    [Test]
    public async Task Returns_all_trackings_for_user()
    {
        var userId = Guid.NewGuid();
        var trackings = new List<TrackingEntry>
        {
            new() { Id = Guid.NewGuid(), LibraryBookId = Guid.NewGuid(), UserId = userId, TotalLength = 300, CurrentIndex = 100 },
            new() { Id = Guid.NewGuid(), LibraryBookId = Guid.NewGuid(), UserId = userId, TotalLength = 420, CurrentIndex = 0 }
        };

        _repository.Setup(r => r.GetUserTrackings(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trackings);

        var result = await _handler.HandleAsync(new GetUserTrackingsQuery(userId), CancellationToken.None);

        result.Trackings.ShouldBe(trackings);
    }

    [Test]
    public async Task Returns_empty_collection_when_user_has_no_trackings()
    {
        var userId = Guid.NewGuid();
        _repository.Setup(r => r.GetUserTrackings(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<TrackingEntry>());

        var result = await _handler.HandleAsync(new GetUserTrackingsQuery(userId), CancellationToken.None);

        result.Trackings.ShouldBeEmpty();
    }
}

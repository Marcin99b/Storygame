using Moq;
using Shouldly;
using Storygame.Cqrs;
using Storygame.Ownership;
using Storygame.Tracking;
using Storygame.Tracking.Commands;
using Storygame.Tracking.Events;
using TrackingEntry = Storygame.Tracking.Tracking;

namespace Storygame.Tests.Unit.Tracking;

[TestFixture]
public class UpdateTrackingIndexCommandHandlerTests
{
    private Mock<ITrackingRepository> _repository = null!;
    private Mock<IDispatcher> _dispatcher = null!;
    private UpdateTrackingIndexCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ITrackingRepository>();
        _dispatcher = new Mock<IDispatcher>();
        _handler = new UpdateTrackingIndexCommandHandler(_repository.Object, _dispatcher.Object);
    }

    [Test]
    public async Task Throws_when_user_is_not_the_owner_of_tracking()
    {
        var tracking = AnyTracking(ownedBy: Guid.NewGuid());
        _repository.Setup(r => r.GetTracking(tracking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tracking);

        var command = new UpdateTrackingIndexCommand(Guid.NewGuid(), tracking.Id, 100);

        await Should.ThrowAsync<OwnershipException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Test]
    public async Task Updates_current_index_to_the_new_value()
    {
        var userId = Guid.NewGuid();
        var tracking = AnyTracking(ownedBy: userId, currentIndex: 50);
        _repository.Setup(r => r.GetTracking(tracking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tracking);

        TrackingEntry? updatedTracking = null;
        _repository.Setup(r => r.UpdateTracking(It.IsAny<TrackingEntry>(), It.IsAny<CancellationToken>()))
            .Callback<TrackingEntry, CancellationToken>((t, _) => updatedTracking = t);

        await _handler.HandleAsync(new UpdateTrackingIndexCommand(userId, tracking.Id, 120), CancellationToken.None);

        updatedTracking.ShouldNotBeNull();
        updatedTracking.CurrentIndex.ShouldBe(120);
    }

    [Test]
    public async Task Publishes_event_with_old_and_new_index()
    {
        var userId = Guid.NewGuid();
        var tracking = AnyTracking(ownedBy: userId, currentIndex: 50);
        _repository.Setup(r => r.GetTracking(tracking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tracking);

        TrackingIndexUpdatedEvent? publishedEvent = null;
        _dispatcher.Setup(d => d.PublishAsync(It.IsAny<TrackingIndexUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<TrackingIndexUpdatedEvent, CancellationToken>((e, _) => publishedEvent = e);

        await _handler.HandleAsync(new UpdateTrackingIndexCommand(userId, tracking.Id, 120), CancellationToken.None);

        publishedEvent.ShouldNotBeNull();
        publishedEvent.TrackingId.ShouldBe(tracking.Id);
        publishedEvent.OldIndex.ShouldBe(50);
        publishedEvent.NewIndex.ShouldBe(120);
    }

    [Test]
    public async Task Does_not_update_repository_when_user_is_not_the_owner()
    {
        var tracking = AnyTracking(ownedBy: Guid.NewGuid());
        _repository.Setup(r => r.GetTracking(tracking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tracking);

        await Should.ThrowAsync<OwnershipException>(() =>
            _handler.HandleAsync(new UpdateTrackingIndexCommand(Guid.NewGuid(), tracking.Id, 100), CancellationToken.None));

        _repository.Verify(r => r.UpdateTracking(It.IsAny<TrackingEntry>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Does_not_publish_event_when_user_is_not_the_owner()
    {
        var tracking = AnyTracking(ownedBy: Guid.NewGuid());
        _repository.Setup(r => r.GetTracking(tracking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tracking);

        await Should.ThrowAsync<OwnershipException>(() =>
            _handler.HandleAsync(new UpdateTrackingIndexCommand(Guid.NewGuid(), tracking.Id, 100), CancellationToken.None));

        _dispatcher.Verify(
            d => d.PublishAsync(It.IsAny<TrackingIndexUpdatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static TrackingEntry AnyTracking(Guid ownedBy, int currentIndex = 0) => new()
    {
        Id = Guid.NewGuid(),
        LibraryBookId = Guid.NewGuid(),
        UserId = ownedBy,
        TotalLength = 412,
        CurrentIndex = currentIndex
    };
}

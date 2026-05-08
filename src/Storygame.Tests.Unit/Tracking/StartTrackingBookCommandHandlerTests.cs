using Moq;
using Shouldly;
using Storygame.Cqrs;
using Storygame.Tracking;
using Storygame.Tracking.Commands;
using Storygame.Tracking.Events;
using TrackingEntry = Storygame.Tracking.Tracking;

namespace Storygame.Tests.Unit.Tracking;

[TestFixture]
public class StartTrackingBookCommandHandlerTests
{
    private Mock<ITrackingRepository> _repository = null!;
    private Mock<IDispatcher> _dispatcher = null!;
    private StartTrackingBookCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ITrackingRepository>();
        _dispatcher = new Mock<IDispatcher>();
        _handler = new StartTrackingBookCommandHandler(_repository.Object, _dispatcher.Object);
    }

    [Test]
    public async Task Throws_when_book_is_already_being_tracked()
    {
        var command = AnyCommand();
        _repository.Setup(r => r.CheckIfBookIsAlreadyTracked(command.LibraryBookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<ArgumentException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Test]
    public async Task Saves_tracking_with_zero_initial_progress()
    {
        var command = AnyCommand();
        _repository.Setup(r => r.CheckIfBookIsAlreadyTracked(command.LibraryBookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        TrackingEntry? savedTracking = null;
        _repository.Setup(r => r.AddTracking(It.IsAny<TrackingEntry>(), It.IsAny<CancellationToken>()))
            .Callback<TrackingEntry, CancellationToken>((t, _) => savedTracking = t);

        await _handler.HandleAsync(command, CancellationToken.None);

        savedTracking.ShouldNotBeNull();
        savedTracking.Id.ShouldNotBe(Guid.Empty);
        savedTracking.LibraryBookId.ShouldBe(command.LibraryBookId);
        savedTracking.UserId.ShouldBe(command.UserId);
        savedTracking.TotalLength.ShouldBe(command.TotalLength);
        savedTracking.CurrentIndex.ShouldBe(0);
    }

    [Test]
    public async Task Publishes_event_with_tracking_data_after_starting()
    {
        var command = AnyCommand();
        _repository.Setup(r => r.CheckIfBookIsAlreadyTracked(command.LibraryBookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        TrackingStartedEvent? publishedEvent = null;
        _dispatcher.Setup(d => d.PublishAsync(It.IsAny<TrackingStartedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<TrackingStartedEvent, CancellationToken>((e, _) => publishedEvent = e);

        await _handler.HandleAsync(command, CancellationToken.None);

        publishedEvent.ShouldNotBeNull();
        publishedEvent.TrackingId.ShouldNotBe(Guid.Empty);
        publishedEvent.LibraryBookId.ShouldBe(command.LibraryBookId);
        publishedEvent.UserId.ShouldBe(command.UserId);
        publishedEvent.TotalLength.ShouldBe(command.TotalLength);
    }

    [Test]
    public async Task Does_not_publish_event_when_book_is_already_tracked()
    {
        var command = AnyCommand();
        _repository.Setup(r => r.CheckIfBookIsAlreadyTracked(command.LibraryBookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<ArgumentException>(() => _handler.HandleAsync(command, CancellationToken.None));

        _dispatcher.Verify(
            d => d.PublishAsync(It.IsAny<TrackingStartedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Does_not_save_tracking_when_book_is_already_tracked()
    {
        var command = AnyCommand();
        _repository.Setup(r => r.CheckIfBookIsAlreadyTracked(command.LibraryBookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<ArgumentException>(() => _handler.HandleAsync(command, CancellationToken.None));

        _repository.Verify(r => r.AddTracking(It.IsAny<TrackingEntry>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static StartTrackingBookCommand AnyCommand()
        => new(Guid.NewGuid(), Guid.NewGuid(), 412);
}

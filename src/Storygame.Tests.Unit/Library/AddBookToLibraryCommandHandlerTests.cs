using Moq;
using Shouldly;
using Storygame.Cqrs;
using Storygame.Library;
using Storygame.Library.Commands;
using Storygame.Library.Events;

namespace Storygame.Tests.Unit.Library;

[TestFixture]
public class AddBookToLibraryCommandHandlerTests
{
    private Mock<ILibraryRepository> _repository = null!;
    private Mock<IDispatcher> _dispatcher = null!;
    private AddBookToLibraryCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ILibraryRepository>();
        _dispatcher = new Mock<IDispatcher>();
        _handler = new AddBookToLibraryCommandHandler(_repository.Object, _dispatcher.Object);
    }

    [Test]
    public async Task Throws_when_user_already_has_the_same_book_in_the_same_format()
    {
        var command = AnyCommand();

        _repository
            .Setup(r => r.CheckIfUserAlreadyHasThisBook(command.UserId, command.CatalogBookId!.Value, command.MediaType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<ArgumentException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Test]
    public async Task Does_not_check_for_duplicates_when_catalog_book_id_is_null()
    {
        var command = AnyCommand() with { CatalogBookId = null };

        await _handler.HandleAsync(command, CancellationToken.None);

        _repository.Verify(
            r => r.CheckIfUserAlreadyHasThisBook(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<MediaType>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Saves_book_with_properties_from_the_command()
    {
        var imageId = Guid.NewGuid();
        var command = new AddBookToLibraryCommand(Guid.NewGuid(), Guid.NewGuid(), imageId, "Dune", "Classic sci-fi", MediaType.Ebook, 412);

        _repository
            .Setup(r => r.CheckIfUserAlreadyHasThisBook(command.UserId, command.CatalogBookId!.Value, command.MediaType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Book? savedBook = null;
        _repository
            .Setup(r => r.AddBook(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .Callback<Book, CancellationToken>((book, _) => savedBook = book);

        await _handler.HandleAsync(command, CancellationToken.None);

        savedBook.ShouldNotBeNull();
        savedBook.Id.ShouldNotBe(Guid.Empty);
        savedBook.UserId.ShouldBe(command.UserId);
        savedBook.CatalogBookId.ShouldBe(command.CatalogBookId);
        savedBook.ImageId.ShouldBe(imageId);
        savedBook.Title.ShouldBe("Dune");
        savedBook.Description.ShouldBe("Classic sci-fi");
        savedBook.MediaType.ShouldBe(MediaType.Ebook);
        savedBook.Length.ShouldBe(412);
        savedBook.AddedToLibraryAt.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
    }

    [Test]
    public async Task Publishes_event_with_data_matching_the_saved_book()
    {
        var command = AnyCommand();

        _repository
            .Setup(r => r.CheckIfUserAlreadyHasThisBook(command.UserId, command.CatalogBookId!.Value, command.MediaType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        BookAddedToLibraryEvent? publishedEvent = null;
        _dispatcher
            .Setup(d => d.PublishAsync(It.IsAny<BookAddedToLibraryEvent>(), It.IsAny<CancellationToken>()))
            .Callback<BookAddedToLibraryEvent, CancellationToken>((e, _) => publishedEvent = e);

        await _handler.HandleAsync(command, CancellationToken.None);

        publishedEvent.ShouldNotBeNull();
        publishedEvent.LibraryBookId.ShouldNotBe(Guid.Empty);
        publishedEvent.UserId.ShouldBe(command.UserId);
        publishedEvent.CatalogBookId.ShouldBe(command.CatalogBookId);
        publishedEvent.Title.ShouldBe(command.Title);
        publishedEvent.MediaType.ShouldBe(command.MediaType);
    }

    [Test]
    public async Task Does_not_publish_event_when_duplicate_is_detected()
    {
        var command = AnyCommand();

        _repository
            .Setup(r => r.CheckIfUserAlreadyHasThisBook(command.UserId, command.CatalogBookId!.Value, command.MediaType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<ArgumentException>(() => _handler.HandleAsync(command, CancellationToken.None));

        _dispatcher.Verify(
            d => d.PublishAsync(It.IsAny<BookAddedToLibraryEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static AddBookToLibraryCommand AnyCommand()
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Dune", "Classic sci-fi", MediaType.Ebook, 412);
}

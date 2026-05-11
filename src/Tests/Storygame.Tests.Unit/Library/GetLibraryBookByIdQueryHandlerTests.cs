using Moq;
using Shouldly;
using Storygame.Library;
using Storygame.Library.Queries;

namespace Storygame.Tests.Unit.Library;

[TestFixture]
public class GetLibraryBookByIdQueryHandlerTests
{
    private Mock<ILibraryRepository> _repository = null!;
    private GetLibraryBookByIdQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ILibraryRepository>();
        _handler = new GetLibraryBookByIdQueryHandler(_repository.Object);
    }

    [Test]
    public async Task Returns_book_for_given_id_and_user()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var book = AnyBook(bookId, userId);

        _repository.Setup(r => r.GetBookById(bookId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.HandleAsync(new GetLibraryBookByIdQuery(bookId, userId), CancellationToken.None);

        result.Book.ShouldBe(book);
    }

    [Test]
    public async Task Passes_both_book_id_and_user_id_to_repository()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _repository.Setup(r => r.GetBookById(bookId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AnyBook(bookId, userId));

        await _handler.HandleAsync(new GetLibraryBookByIdQuery(bookId, userId), CancellationToken.None);

        _repository.Verify(r => r.GetBookById(bookId, userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Book AnyBook(Guid id, Guid userId) => new()
    {
        Id = id, UserId = userId, CatalogBookId = null, ImageId = null,
        Title = "Dune", Description = "Classic sci-fi", MediaType = MediaType.Ebook,
        Length = 412, AddedToLibraryAt = DateTime.UtcNow
    };
}

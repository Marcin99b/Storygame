using Moq;
using Shouldly;
using Storygame.Library;
using Storygame.Library.Queries;

namespace Storygame.Tests.Unit.Library;

[TestFixture]
public class GetUserBooksFromLibraryQueryHandlerTests
{
    private Mock<ILibraryRepository> _repository = null!;
    private GetUserBooksFromLibraryQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ILibraryRepository>();
        _handler = new GetUserBooksFromLibraryQueryHandler(_repository.Object);
    }

    [Test]
    public async Task Returns_all_books_belonging_to_user()
    {
        var userId = Guid.NewGuid();
        var books = new List<Book> { AnyBook(userId), AnyBook(userId) };

        _repository.Setup(r => r.GetUserBooks(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var result = await _handler.HandleAsync(new GetUserBooksFromLibraryQuery(userId), CancellationToken.None);

        result.Books.ShouldBe(books);
    }

    [Test]
    public async Task Returns_empty_collection_when_user_has_no_books()
    {
        var userId = Guid.NewGuid();
        _repository.Setup(r => r.GetUserBooks(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Book>());

        var result = await _handler.HandleAsync(new GetUserBooksFromLibraryQuery(userId), CancellationToken.None);

        result.Books.ShouldBeEmpty();
    }

    private static Book AnyBook(Guid userId) => new()
    {
        Id = Guid.NewGuid(), UserId = userId, CatalogBookId = null, ImageId = null,
        Title = "Dune", Description = "Classic sci-fi", MediaType = MediaType.Ebook,
        Length = 412, AddedToLibraryAt = DateTime.UtcNow
    };
}

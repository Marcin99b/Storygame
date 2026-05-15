using Storygame.Contracts.WebApi.Dtos;
using Storygame.Contracts.WebApi.Requests;
using System.Net;

namespace Storygame.Tests.Integration.Library;

[TestFixture]
public class LibraryTests : AuthenticatedIntegrationTestBase
{
    [Test]
    public async Task GetLibrary_Initially_ReturnsEmptyList()
    {
        var response = await Client.GetLibrary();

        response.Books.ShouldBeEmpty();
    }

    [Test]
    public async Task AddToLibrary_AddsBook_VisibleInGetLibrary()
    {
        await Client.AddToLibrary(new AddToLibraryRequest(null, null, "Dune", "Sci-fi epic", MediaTypeDto.Ebook, 412));

        var library = await Client.GetLibrary();

        library.Books.ShouldHaveSingleItem();
        var book = library.Books[0];
        book.Title.ShouldBe("Dune");
        book.Description.ShouldBe("Sci-fi epic");
        book.MediaType.ShouldBe(MediaTypeDto.Ebook);
        book.Length.ShouldBe(412);
    }

    [Test]
    public async Task AddToLibrary_MultipleBooksWithNoCatalogId_AllAdded()
    {
        await Client.AddToLibrary(new AddToLibraryRequest(null, null, "Book A", "Desc A", MediaTypeDto.Ebook, 100));
        await Client.AddToLibrary(new AddToLibraryRequest(null, null, "Book B", "Desc B", MediaTypeDto.Audiobook, 200));

        var library = await Client.GetLibrary();

        library.Books.Length.ShouldBe(2);
    }

    [Test]
    public async Task AddToLibrary_DuplicateCatalogBook_Fails()
    {
        var catalogBookId = Guid.NewGuid();
        await Client.AddToLibrary(new AddToLibraryRequest(catalogBookId, null, "Dune", "Sci-fi epic", MediaTypeDto.Ebook, 412));

        await Should.ThrowAsync<HttpRequestException>(() =>
            Client.AddToLibrary(new AddToLibraryRequest(catalogBookId, null, "Dune", "Sci-fi epic", MediaTypeDto.Ebook, 412)));
    }

    [Test]
    public async Task GetLibrary_WhenUnauthenticated_Returns401()
    {
        var status = await GetUnauthenticated("/api/library/");

        status.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

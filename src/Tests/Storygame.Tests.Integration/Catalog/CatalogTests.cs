using System.Net;

namespace Storygame.Tests.Integration.Catalog;

[TestFixture]
public class CatalogTests : AuthenticatedIntegrationTestBase
{
    [Test]
    public async Task GetCatalog_ReturnsAllBooks()
    {
        var response = await Client.GetCatalog();

        response.Books.ShouldNotBeEmpty();
    }

    [Test]
    public async Task GetCatalog_FilterByTitle_ReturnsMatchingBooks()
    {
        var response = await Client.GetCatalog(titleContains: "C#");

        response.Books.ShouldNotBeEmpty();
        response.Books.ShouldAllBe(b => b.Title.Contains("C#"));
    }

    [Test]
    public async Task GetCatalog_FilterByAudiobook_ReturnsOnlyAudiobooks()
    {
        var response = await Client.GetCatalog(hasAudiobook: true);

        response.Books.ShouldNotBeEmpty();
        response.Books.ShouldAllBe(b => b.AudiobookFields.Exist);
    }

    [Test]
    public async Task GetCatalog_FilterByNoAudiobook_ReturnsOnlyTextEditions()
    {
        var response = await Client.GetCatalog(hasAudiobook: false);

        response.Books.ShouldNotBeEmpty();
        response.Books.ShouldAllBe(b => !b.AudiobookFields.Exist);
    }

    [Test]
    public async Task GetCatalog_WhenUnauthenticated_Returns401()
    {
        var status = await GetUnauthenticated("/api/catalog");

        status.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

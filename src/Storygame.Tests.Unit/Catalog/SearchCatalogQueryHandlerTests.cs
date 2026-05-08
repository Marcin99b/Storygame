using Shouldly;
using Storygame.Catalog.Queries;

namespace Storygame.Tests.Unit.Catalog;

[TestFixture]
public class SearchCatalogQueryHandlerTests
{
    private SearchCatalogQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = new SearchCatalogQueryHandler();
    }

    [Test]
    public async Task Returns_all_books_when_no_filters_are_applied()
    {
        var result = await _handler.HandleAsync(new SearchCatalogQuery(), CancellationToken.None);

        result.Books.Count().ShouldBe(12);
    }

    [Test]
    public async Task Filters_books_by_title_substring()
    {
        var result = await _handler.HandleAsync(new SearchCatalogQuery(TitleContains: "C#"), CancellationToken.None);

        result.Books.Count().ShouldBe(2);
        result.Books.ShouldAllBe(b => b.Title.Contains("C#"));
    }

    [Test]
    public async Task Returns_only_books_with_audiobook_edition()
    {
        var result = await _handler.HandleAsync(new SearchCatalogQuery(HasAudiobook: true), CancellationToken.None);

        result.Books.ShouldNotBeEmpty();
        result.Books.ShouldAllBe(b => b.AudiobookFields.Exist);
    }

    [Test]
    public async Task Returns_only_books_without_audiobook_edition()
    {
        var result = await _handler.HandleAsync(new SearchCatalogQuery(HasAudiobook: false), CancellationToken.None);

        result.Books.ShouldNotBeEmpty();
        result.Books.ShouldAllBe(b => !b.AudiobookFields.Exist);
    }

    [Test]
    public async Task Audiobook_and_non_audiobook_results_sum_to_all_books()
    {
        var withAudiobook = await _handler.HandleAsync(new SearchCatalogQuery(HasAudiobook: true), CancellationToken.None);
        var withoutAudiobook = await _handler.HandleAsync(new SearchCatalogQuery(HasAudiobook: false), CancellationToken.None);
        var all = await _handler.HandleAsync(new SearchCatalogQuery(), CancellationToken.None);

        (withAudiobook.Books.Count() + withoutAudiobook.Books.Count()).ShouldBe(all.Books.Count());
    }

    [Test]
    public async Task Returns_empty_when_title_filter_matches_nothing()
    {
        var result = await _handler.HandleAsync(
            new SearchCatalogQuery(TitleContains: "DefinitelyNotInCatalog"), CancellationToken.None);

        result.Books.ShouldBeEmpty();
    }

    [Test]
    public async Task Combines_title_and_audiobook_filters()
    {
        var result = await _handler.HandleAsync(
            new SearchCatalogQuery(TitleContains: "The", HasAudiobook: true), CancellationToken.None);

        result.Books.ShouldAllBe(b => b.Title.Contains("The") && b.AudiobookFields.Exist);
    }

    [Test]
    public async Task Throws_when_cancellation_is_requested_before_execution()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(new SearchCatalogQuery(), cts.Token));
    }
}

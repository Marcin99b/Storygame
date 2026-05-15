using Storygame.Contracts.WebApi.Dtos;
using Storygame.Contracts.WebApi.Requests;
using Storygame.Contracts.WebApi.Responses;
using System.Net;

namespace Storygame.Tests.Integration.Tracking;

[TestFixture]
public class TrackingTests : AuthenticatedIntegrationTestBase
{
    [Test]
    public async Task GetTrackings_Initially_ReturnsEmptyList()
    {
        var response = await Client.GetTrackings();

        response.Trackings.ShouldBeEmpty();
    }

    [Test]
    public async Task StartTracking_CreatesTracking()
    {
        var libraryBookId = await AddBookToLibraryAndGetId();

        await Client.StartTracking(new StartTrackingRequest(libraryBookId));
        var trackings = await Client.GetTrackings();

        trackings.Trackings.ShouldHaveSingleItem();
        var tracking = trackings.Trackings[0];
        tracking.LibraryBookId.ShouldBe(libraryBookId);
        tracking.CurrentIndex.ShouldBe(0);
        tracking.IsStarted.ShouldBeFalse();
        tracking.IsFinished.ShouldBeFalse();
    }

    [Test]
    public async Task StartTracking_SameBookTwice_Fails()
    {
        var libraryBookId = await AddBookToLibraryAndGetId();
        await Client.StartTracking(new StartTrackingRequest(libraryBookId));

        await Should.ThrowAsync<HttpRequestException>(() =>
            Client.StartTracking(new StartTrackingRequest(libraryBookId)));
    }

    [Test]
    public async Task UpdateIndex_UpdatesCurrentIndex()
    {
        var libraryBookId = await AddBookToLibraryAndGetId();
        await Client.StartTracking(new StartTrackingRequest(libraryBookId));
        var trackingId = (await Client.GetTrackings()).Trackings[0].Id;

        await Client.UpdateIndex(trackingId, new UpdateIndexRequest(50));
        var trackings = await Client.GetTrackings();

        trackings.Trackings[0].CurrentIndex.ShouldBe(50);
        trackings.Trackings[0].IsStarted.ShouldBeTrue();
    }

    [Test]
    public async Task UpdateIndex_ToTotalLength_MarksAsFinished()
    {
        var libraryBookId = await AddBookToLibraryAndGetId(length: 100);
        await Client.StartTracking(new StartTrackingRequest(libraryBookId));
        var trackingId = (await Client.GetTrackings()).Trackings[0].Id;

        await Client.UpdateIndex(trackingId, new UpdateIndexRequest(100));
        var trackings = await Client.GetTrackings();

        trackings.Trackings[0].IsFinished.ShouldBeTrue();
    }

    [Test]
    public async Task GetStatistics_ReturnsEmptyBeforeAnyIndexUpdate()
    {
        var libraryBookId = await AddBookToLibraryAndGetId();
        await Client.StartTracking(new StartTrackingRequest(libraryBookId));
        var trackingId = (await Client.GetTrackings()).Trackings[0].Id;

        var stats = await Client.GetStatistics(trackingId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, TimePeriodDto.Day);

        stats.TrackingStatistics.ShouldBeEmpty();
    }

    [Test]
    public async Task GetStatistics_AfterIndexUpdate_ReturnsStatistics()
    {
        var libraryBookId = await AddBookToLibraryAndGetId();
        await Client.StartTracking(new StartTrackingRequest(libraryBookId));
        var trackingId = (await Client.GetTrackings()).Trackings[0].Id;

        await Client.UpdateIndex(trackingId, new UpdateIndexRequest(30));
        await Task.Delay(1500);

        var stats = await Client.GetStatistics(trackingId, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), TimePeriodDto.Day);

        stats.TrackingStatistics.ShouldNotBeEmpty();
        stats.TrackingStatistics[0].Value.ShouldBe(30);
        stats.TrackingStatistics[0].TimePeriod.ShouldBe(TimePeriodDto.Day);
    }

    [Test]
    public async Task GetTrackings_WhenUnauthenticated_Returns401()
    {
        var status = await GetUnauthenticated("/api/tracking/");

        status.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private async Task<Guid> AddBookToLibraryAndGetId(int length = 300)
    {
        await Client.AddToLibrary(new AddToLibraryRequest(null, null, "Test Book", "A test book", MediaTypeDto.Ebook, length));
        return (await Client.GetLibrary()).Books[0].Id;
    }
}

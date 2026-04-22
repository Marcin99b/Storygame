using Storygame.Client;
using Storygame.Contracts.WebApi;

public static class Scenarios
{
    public static async Task SimpleScenario(Uri url)
    {
        var client = new StorygameClient(url);
        await client.Login();
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 3)));
        var me = await client.Me();
        var catalog = await client.GetCatalog();
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(5, 10)));
        var book1 = catalog.Books[0];

        await client.AddToLibrary(new(
            book1.Id,
            book1.ImageId,
            book1.Title,
            book1.Description,
            MediaTypeDto.Audiobook,
            book1.AudiobookFields.TotalMinutes));
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 5)));

        var library = await client.GetLibrary();
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(2, 4)));

        var libraryBook1 = library.Books[0];

        await client.StartTracking(new(libraryBook1.Id, libraryBook1.Length));
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(15, 60)));
        var trackings = await client.GetTrackings();
        var tracking1 = trackings.Trackings[0];

        await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(5));
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(15, 60)));
        await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(15));
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(15, 60)));
        await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(25));
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(15, 60)));
        await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(30));
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(15, 60)));
        await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(50));
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(15, 60)));

        var updatedTrackings = await client.GetTrackings();
    }
}
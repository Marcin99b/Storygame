using Storygame.Client;
using Storygame.Contracts.WebApi;

var url = new Uri("https://localhost:7121");
var client = new StorygameClient(url);

await client.Login();
var me = await client.Me();
var catalog = await client.GetCatalog();
var book1 = catalog.Books[0];

await client.AddToLibrary(new AddToLibraryRequest(
    book1.Id, 
    book1.ImageId, 
    book1.Title, 
    book1.Description,
    MediaTypeDto.Audiobook, 
    book1.AudiobookFields.TotalMinutes));

var library = await client.GetLibrary();

var libraryBook1 = library.Books[0];

await client.StartTracking(new StartTrackingRequest(libraryBook1.Id, libraryBook1.Length));

var trackings = await client.GetTrackings();
var tracking1 = trackings.Trackings[0];

await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(5));
await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(15));
await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(25));
await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(30));
await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(50));

var updatedTrackings = await client.GetTrackings();

await Task.Delay(-1);
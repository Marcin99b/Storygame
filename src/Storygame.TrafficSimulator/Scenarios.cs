using System;
using System.Linq;
using System.Threading.Tasks;
using Storygame.Client;
using Storygame.Contracts.WebApi;

public static class Scenarios
{
    private static readonly Random Rng = new();

    // Helper: delay for a random number of seconds in [min, max)
    private static Task RandomDelay(int minSeconds, int maxSeconds)
        => Task.Delay(TimeSpan.FromSeconds(Rng.Next(minSeconds, maxSeconds)));

    // Helper: pick a random element or null
    private static T? PickRandom<T>(T[] items) where T : class
        => items.Length == 0 ? null : items[Rng.Next(0, items.Length)];

    // Keep the original simple scenario but use shared RNG and helper utilities
    public static async Task SimpleScenario(Uri url)
    {
        var client = new StorygameClient(url);
        await CreateUserAndLogin(client);
        await RandomDelay(1, 3);

        var me = await client.Me();
        var catalog = await client.GetCatalog();
        await RandomDelay(5, 10);

        var book1 = catalog.Books.FirstOrDefault();
        if (book1 is null)
            return;

        // Prefer audiobook length when possible
        var length = book1.AudiobookFields.Exist ? book1.AudiobookFields.TotalMinutes : book1.TextEditionFields.TotalPages;

        await client.AddToLibrary(new(
            book1.Id,
            book1.ImageId,
            book1.Title,
            book1.Description,
            book1.AudiobookFields.Exist ? MediaTypeDto.Audiobook : MediaTypeDto.Ebook,
            length));

        await RandomDelay(1, 5);

        var library = await client.GetLibrary();
        await RandomDelay(2, 4);

        var libraryBook1 = library.Books.FirstOrDefault();
        if (libraryBook1 is null)
            return;

        await client.StartTracking(new(libraryBook1.Id));

        // A few periodic updates representing listening/reading sessions
        for (var i = 0; i < 5; i++)
        {
            await RandomDelay(15, 60);
            var trackings = await client.GetTrackings();
            var tracking1 = trackings.Trackings.FirstOrDefault(t => t.LibraryBookId == libraryBook1.Id) ?? trackings.Trackings.FirstOrDefault();
            if (tracking1 is null) break;

            // move forward by a small random amount
            var increment = Math.Min(libraryBook1.Length, tracking1.CurrentIndex + Rng.Next(1, Math.Max(2, libraryBook1.Length / 10)));
            await client.UpdateIndex(tracking1.Id, new UpdateIndexRequest(increment));
        }
    }

    // Browsing scenario: browse many books and add a couple to library
    public static async Task BrowsingAndAddScenario(Uri url)
    {
        var client = new StorygameClient(url);
        await CreateUserAndLogin(client);
        await RandomDelay(1, 4);

        var catalog = await client.GetCatalog();

        // Simulate a user flipping through a catalog: 3..8 books
        var toInspect = Rng.Next(3, Math.Min(9, catalog.Books.Length + 1));
        for (var i = 0; i < toInspect; i++)
        {
            var book = catalog.Books[Rng.Next(0, catalog.Books.Length)];
            // dwell time on a book: 2..12 seconds
            await RandomDelay(2, 12);

            // occasionally add inspected book to library (30% chance)
            if (Rng.NextDouble() < 0.3)
            {
                var media = book.AudiobookFields.Exist ? MediaTypeDto.Audiobook : MediaTypeDto.Ebook;
                var length = book.AudiobookFields.Exist ? book.AudiobookFields.TotalMinutes : book.TextEditionFields.TotalPages;
                await client.AddToLibrary(new(book.Id, book.ImageId, book.Title, book.Description, media, length));
                // short break after adding
                await RandomDelay(1, 4);
            }
        }
    }

    // Audiobook listener: starts an audiobook and simulates multiple listening sessions
    public static async Task AudiobookListeningScenario(Uri url)
    {
        var client = new StorygameClient(url);
        await CreateUserAndLogin(client);
        await RandomDelay(1, 3);

        var catalog = await client.GetCatalog();
        var audiobooks = catalog.Books.Where(b => b.AudiobookFields.Exist).ToArray();
        var chosen = PickRandom(audiobooks);
        if (chosen is null)
            return;

        var length = chosen.AudiobookFields.TotalMinutes;
        await client.AddToLibrary(new(chosen.Id, chosen.ImageId, chosen.Title, chosen.Description, MediaTypeDto.Audiobook, length));
        await RandomDelay(1, 6);

        var library = await client.GetLibrary();
        var libBook = library.Books.FirstOrDefault(b => b.CatalogBookId == chosen.Id) ?? library.Books.FirstOrDefault(b => b.MediaType == MediaTypeDto.Audiobook);
        if (libBook is null) return;

        await client.StartTracking(new(libBook.Id));

        // Simulate daily listening sessions for several days (1..5 sessions)
        var sessions = Rng.Next(1, 6);
        for (var s = 0; s < sessions; s++)
        {
            // session length 10..90 minutes
            var minutes = Rng.Next(10, Math.Min(90, libBook.Length / Math.Max(1, sessions)));
            await RandomDelay(60, 60 * (minutes / 1)); // convert to seconds (approximate)

            var trackings = await client.GetTrackings();
            var tracking = trackings.Trackings.FirstOrDefault(t => t.LibraryBookId == libBook.Id);
            if (tracking is null) break;

            var newIndex = Math.Min(libBook.Length, tracking.CurrentIndex + minutes);
            await client.UpdateIndex(tracking.Id, new UpdateIndexRequest(newIndex));

            // longer break between days
            await RandomDelay(60 * 60 * 6, 60 * 60 * 24); // 6 hours .. 24 hours
        }
    }

    // Binge reader: picks a text edition and reads many pages quickly
    public static async Task BingeReaderScenario(Uri url)
    {
        var client = new StorygameClient(url);
        await CreateUserAndLogin(client);
        await RandomDelay(1, 2);

        var catalog = await client.GetCatalog();
        var texts = catalog.Books.Where(b => b.TextEditionFields.Exist).ToArray();
        var chosen = PickRandom(texts);
        if (chosen is null) return;

        var length = chosen.TextEditionFields.TotalPages;
        await client.AddToLibrary(new(chosen.Id, chosen.ImageId, chosen.Title, chosen.Description, MediaTypeDto.Paperback, length));
        await RandomDelay(1, 3);

        var library = await client.GetLibrary();
        var libBook = library.Books.FirstOrDefault(b => b.CatalogBookId == chosen.Id) ?? library.Books.FirstOrDefault(b => b.MediaType != MediaTypeDto.Audiobook);
        if (libBook is null) return;

        await client.StartTracking(new(libBook.Id));

        // Rapid reading: several fast updates
        var updates = Rng.Next(3, 10);
        for (var i = 0; i < updates; i++)
        {
            await RandomDelay(5, 30);
            var trackings = await client.GetTrackings();
            var track = trackings.Trackings.FirstOrDefault(t => t.LibraryBookId == libBook.Id);
            if (track is null) break;

            var remaining = libBook.Length - track.CurrentIndex;
            if (remaining <= 0) break;

            var step = Math.Min(remaining, Rng.Next(Math.Max(1, libBook.Length / 8), Math.Max(2, libBook.Length / 3)));
            var newIndex = Math.Min(libBook.Length, track.CurrentIndex + step);
            await client.UpdateIndex(track.Id, new UpdateIndexRequest(newIndex));
        }
    }

    // Casual sampler: starts many different books for a short time
    public static async Task CasualSamplerScenario(Uri url)
    {
        var client = new StorygameClient(url);
        await CreateUserAndLogin(client);
        await RandomDelay(1, 3);

        var catalog = await client.GetCatalog();

        // inspect 5..12 books and occasionally add then start a short tracking session
        var inspect = Rng.Next(5, Math.Min(13, catalog.Books.Length + 1));
        for (var i = 0; i < inspect; i++)
        {
            var book = catalog.Books[Rng.Next(0, catalog.Books.Length)];
            await RandomDelay(1, 8);

            if (Rng.NextDouble() < 0.2)
            {
                var media = book.AudiobookFields.Exist ? MediaTypeDto.Audiobook : MediaTypeDto.Ebook;
                var length = book.AudiobookFields.Exist ? book.AudiobookFields.TotalMinutes : book.TextEditionFields.TotalPages;
                await client.AddToLibrary(new(book.Id, book.ImageId, book.Title, book.Description, media, length));
                await RandomDelay(1, 4);

                var library = await client.GetLibrary();
                var libBook = library.Books.FirstOrDefault(b => b.CatalogBookId == book.Id);
                if (libBook != null)
                {
                    await client.StartTracking(new(libBook.Id));
                    await RandomDelay(10, 120);
                    var trackings = await client.GetTrackings();
                    var tracking = trackings.Trackings.FirstOrDefault(t => t.LibraryBookId == libBook.Id);
                    if (tracking != null)
                    {
                        var progress = Math.Min(libBook.Length, tracking.CurrentIndex + Rng.Next(1, Math.Max(2, libBook.Length / 20)));
                        await client.UpdateIndex(tracking.Id, new UpdateIndexRequest(progress));
                    }
                }
            }
        }
    }

    // Abandon scenario: user adds a book and starts tracking but never really progresses
    public static async Task AbandonScenario(Uri url)
    {
        var client = new StorygameClient(url);
        await CreateUserAndLogin(client);
        await RandomDelay(1, 3);

        var catalog = await client.GetCatalog();
        var chosen = PickRandom(catalog.Books);
        if (chosen is null) return;

        var media = chosen.AudiobookFields.Exist ? MediaTypeDto.Audiobook : MediaTypeDto.Ebook;
        var length = chosen.AudiobookFields.Exist ? chosen.AudiobookFields.TotalMinutes : chosen.TextEditionFields.TotalPages;
        await client.AddToLibrary(new(chosen.Id, chosen.ImageId, chosen.Title, chosen.Description, media, length));
        await RandomDelay(1, 5);

        var library = await client.GetLibrary();
        var libBook = library.Books.FirstOrDefault(b => b.CatalogBookId == chosen.Id);
        if (libBook is null) return;

        await client.StartTracking(new(libBook.Id));
    }

    public static async Task CreateUserAndLogin(StorygameClient client)
    {
        var name = "username_" + Guid.NewGuid().ToString();
        var email = "email_" + Guid.NewGuid() + "@example.com";
        await client.Register(new RegisterRequest(name, email));
        var mails = await client.Mail(email);
        var latestMail = mails.OrderByDescending(x => x.SentAt).First();
        var verificationKey = latestMail.Message;

    }
}

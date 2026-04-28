using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Storygame.Catalog;

namespace Storygame.Catalog.Queries;

public record SearchCatalogQuery(
    string? TitleContains = null, 
    bool? HasTextEdition = null, 
    bool? HasAudiobook = null) 
    : IQuery<SearchCatalogQueryResult>;
public record SearchCatalogQueryResult(IEnumerable<Book> Books);

public class SearchCatalogQueryHandler : IQueryHandler<SearchCatalogQuery, SearchCatalogQueryResult>
{
    private readonly IEnumerable<Book> memoryBooks = new List<Book>()
    {
        // todo: move to collection in mongodb
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = null,
            Title = "The Lost Kingdom",
            Description = "An epic fantasy of adventure and discovery.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 412 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 720 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = Guid.NewGuid(),
            Title = "Learning C#",
            Description = "A practical guide to modern C# programming for developers.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 320 },
            AudiobookFields = new AudiobookFields { Exist = false, TotalMinutes = 0 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = Guid.NewGuid(),
            Title = "Short Stories Collection",
            Description = "A curated anthology of contemporary short fiction.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 176 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 95 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = Guid.NewGuid(),
            Title = "The Silent Patient",
            Description = "A tense psychological thriller about a woman's refusal to speak after a violent act.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 336 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 420 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = Guid.NewGuid(),
            Title = "The Midnight Library",
            Description = "A novel exploring choices, regrets and the many possible lives one could live.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 304 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 360 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = null,
            Title = "Effective C#",
            Description = "Practical techniques and patterns for writing robust C# code.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 256 },
            AudiobookFields = new AudiobookFields { Exist = false, TotalMinutes = 0 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = Guid.NewGuid(),
            Title = "Cooking at Home: Recipes for Every Day",
            Description = "A collection of reliable, seasonal recipes for daily home cooking.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 240 },
            AudiobookFields = new AudiobookFields { Exist = false, TotalMinutes = 0 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = null,
            Title = "History of Europe: From the Renaissance to the 20th Century",
            Description = "A comprehensive survey of European history covering major political and cultural changes.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 688 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 1200 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = Guid.NewGuid(),
            Title = "Children's Bedtime Stories",
            Description = "Short, illustrated stories designed to read aloud at bedtime.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 64 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 40 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = null,
            Title = "Modern Romance",
            Description = "An accessible look at contemporary love, dating and relationships.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 288 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 360 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = Guid.NewGuid(),
            Title = "Spaceborne",
            Description = "A hard sci-fi novel following a crew on a deep-space exploration mission.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 420 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 900 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = null,
            Title = "Photography Basics",
            Description = "An illustrated guide to the fundamentals of digital photography and composition.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 176 },
            AudiobookFields = new AudiobookFields { Exist = false, TotalMinutes = 0 }
        }
    };

    public Task<SearchCatalogQueryResult> HandleAsync(SearchCatalogQuery query, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var filtered = memoryBooks;
        if (query.TitleContains != null)
        {
            filtered = filtered.Where(x => x.Title.Contains(query.TitleContains));
        }
        if (query.HasTextEdition.HasValue)
        {
            filtered = filtered.Where(x => x.TextEditionFields.Exist == query.HasTextEdition.Value);
        }
        if (query.HasAudiobook.HasValue)
        {
            filtered = filtered.Where(x => x.AudiobookFields.Exist == query.HasAudiobook.Value);
        }

        var array = filtered.ToArray();
        var result = new SearchCatalogQueryResult(array);
        return Task.FromResult(result);
    }
}

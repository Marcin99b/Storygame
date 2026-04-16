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
        //todo: move to collection in mongodb
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
            ImageId = null,
            Title = "Learning C#",
            Description = "A practical guide to modern C# programming.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 280 },
            AudiobookFields = new AudiobookFields { Exist = false, TotalMinutes = 0 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            ImageId = null,
            Title = "Short Stories Collection",
            Description = "A collection of short fictional pieces.",
            TextEditionFields = new TextEditionFields { Exist = false, TotalPages = 0 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 95 }
        }
    };

    public Task<SearchCatalogQueryResult> HandleAsync(SearchCatalogQuery query)
    {
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

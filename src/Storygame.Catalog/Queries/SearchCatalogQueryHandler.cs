using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Storygame.Catalog;

namespace Storygame.Catalog.Queries;

public record SearchCatalogQuery(
    string? titleContains = null, 
    bool? hasTextEdition = null, 
    bool? hasAudiobook = null) 
    : IQuery<SearchCatalogQueryResult>;
public record SearchCatalogQueryResult(IEnumerable<Book> Books);

public class SearchCatalogQueryHandler : IQueryHandler<SearchCatalogQuery, SearchCatalogQueryResult>
{
    private readonly IEnumerable<Book> memoryBooks = new List<Book>()
    {
        new Book
        {
            Id = Guid.NewGuid(),
            Title = "The Lost Kingdom",
            Description = "An epic fantasy of adventure and discovery.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 412 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 720 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            Title = "Learning C#",
            Description = "A practical guide to modern C# programming.",
            TextEditionFields = new TextEditionFields { Exist = true, TotalPages = 280 },
            AudiobookFields = new AudiobookFields { Exist = false, TotalMinutes = 0 }
        },
        new Book
        {
            Id = Guid.NewGuid(),
            Title = "Short Stories Collection",
            Description = "A collection of short fictional pieces.",
            TextEditionFields = new TextEditionFields { Exist = false, TotalPages = 0 },
            AudiobookFields = new AudiobookFields { Exist = true, TotalMinutes = 95 }
        }
    };

    public Task<SearchCatalogQueryResult> HandleAsync(SearchCatalogQuery query)
    {
        var filtered = memoryBooks.Where(x =>
        {
            if (query.titleContains != null && !x.Title.Contains(query.titleContains))
            {
                return false;
            }

            if (query.hasTextEdition.HasValue && x.TextEditionFields.Exist != query.hasTextEdition.Value)
            {
                return false;
            }

            if (query.hasAudiobook.HasValue && x.AudiobookFields.Exist != query.hasAudiobook.Value)
            {
                return false;
            }

            return true;
        }).ToArray();

        var result = new SearchCatalogQueryResult(filtered);
        return Task.FromResult(result);
    }
}

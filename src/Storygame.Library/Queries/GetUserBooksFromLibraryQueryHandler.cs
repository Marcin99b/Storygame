using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Queries;

public record GetUserBooksFromLibraryQuery(Guid UserId) : IQuery<GetUserBooksFromLibraryQueryResult>;
public record GetUserBooksFromLibraryQueryResult(IEnumerable<Book> Books);

public class GetUserBooksFromLibraryQueryHandler(ILibraryRepository libraryRepository) : IQueryHandler<GetUserBooksFromLibraryQuery, GetUserBooksFromLibraryQueryResult>
{
    public async Task<GetUserBooksFromLibraryQueryResult> HandleAsync(GetUserBooksFromLibraryQuery query, CancellationToken ct)
    {
        var books = await libraryRepository.GetUserBooks(query.UserId, ct);
        return new GetUserBooksFromLibraryQueryResult(books);
    }
}

using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Queries;

public record GetLibraryBookByIdQuery(Guid LibraryBookId, Guid UserId) : IQuery<GetLibraryBookByIdQueryResult>;
public record GetLibraryBookByIdQueryResult(Book Book);

public class GetLibraryBookByIdQueryHandler(ILibraryRepository libraryRepository) : IQueryHandler<GetLibraryBookByIdQuery, GetLibraryBookByIdQueryResult>
{
    public async Task<GetLibraryBookByIdQueryResult> HandleAsync(GetLibraryBookByIdQuery query, CancellationToken ct)
    {
        var book = await libraryRepository.GetBookById(query.LibraryBookId, query.UserId, ct);
        return new GetLibraryBookByIdQueryResult(book);
    }
}

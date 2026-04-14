using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Queries;

public record GetUserBooksFromLibraryQuery(Guid UserId) : IQuery<GetUserBooksFromLibraryQueryResult>;
public record GetUserBooksFromLibraryQueryResult;

public class GetUserBooksFromLibraryQueryHandler(ILibraryRepository libraryRepository) : IQueryHandler<GetUserBooksFromLibraryQuery, GetUserBooksFromLibraryQueryResult>
{
    public async Task<GetUserBooksFromLibraryQueryResult> HandleAsync(GetUserBooksFromLibraryQuery query)
    {
        var books = libraryRepository.GetUserBooks(query.UserId);
        return new GetUserBooksFromLibraryQueryResult();
    }
}

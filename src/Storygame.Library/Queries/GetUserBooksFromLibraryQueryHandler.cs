using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Queries;

public record GetUserBooksFromLibraryQuery : IQuery<GetUserBooksFromLibraryQueryResult>;
public record GetUserBooksFromLibraryQueryResult;

public class GetUserBooksFromLibraryQueryHandler : IQueryHandler<GetUserBooksFromLibraryQuery, GetUserBooksFromLibraryQueryResult>
{
    public Task<GetUserBooksFromLibraryQueryResult> HandleAsync(GetUserBooksFromLibraryQuery query)
    {
        return Task.FromResult(new GetUserBooksFromLibraryQueryResult());
    }
}

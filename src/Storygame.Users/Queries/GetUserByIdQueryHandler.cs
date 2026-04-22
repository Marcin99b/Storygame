using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IQuery<GetUserByIdQueryResult>;
public record GetUserByIdQueryResult(User User);

public class GetUserByIdQueryHandler(IUsersRepository usersRepository) : IQueryHandler<GetUserByIdQuery, GetUserByIdQueryResult>
{
    public async Task<GetUserByIdQueryResult> HandleAsync(GetUserByIdQuery query)
    {
        var user = await usersRepository.GetUserById(query.UserId);
        return new GetUserByIdQueryResult(user);
    }
}

using Storygame.Cqrs;

namespace Storygame.Users.Queries;

public record GetUserByEmailQuery(string Email) : IQuery<GetUserByEmailQueryResult>;
public record GetUserByEmailQueryResult(User User);

public class GetUserByEmailQueryHandler(IUsersRepository usersRepository) : IQueryHandler<GetUserByEmailQuery, GetUserByEmailQueryResult>
{
    public async Task<GetUserByEmailQueryResult> HandleAsync(GetUserByEmailQuery query)
    {
        var user = await usersRepository.GetUserByEmail(query.Email);
        return new GetUserByEmailQueryResult(user);
    }
}
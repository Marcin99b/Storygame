namespace Storygame.Web.Areas.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", GetUsers);
        group.MapGet("/{id:guid}", GetUserById);

        return app;
    }

    public static Task GetUsers() => Task.CompletedTask;
    public static Task GetUserById() => Task.CompletedTask;
}

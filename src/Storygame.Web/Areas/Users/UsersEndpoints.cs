using DnsClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Storygame.Users;
using System.Security.Claims;

namespace Storygame.Web.Areas.Users;

public static class UsersEndpoints
{
    private static Guid currentUser = new Guid("56f95c72-24a4-48d5-88fc-d715288c51d5");
    public record MeResponse(string Name, bool IsVerified);

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization();

        group.MapGet("/Me", GetMe);
        group.MapPost("/Login", Login).AllowAnonymous();
        group.MapPost("/Logout", Logout);

        return app;
    }

    public static Task GetMe() => Task.FromResult(new MeResponse("TestUser", true));
    public static async Task Login(HttpContext http)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, currentUser.ToString()),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            AllowRefresh = true,
        };

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
    }

    public static Task Logout(HttpContext http) => http.SignOutAsync();
}

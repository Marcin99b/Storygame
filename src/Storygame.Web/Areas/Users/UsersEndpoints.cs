using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Storygame.Contracts.WebApi;
using Storygame.Users;
using Storygame.Web.Auth;
using System.Security.Claims;

namespace Storygame.Web.Areas.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization(AuthExtensions.ActionIsRequestedByUserPolicy);

        group.MapGet("/Me", GetMe);
        group.MapPost("/Login", Login).AllowAnonymous();
        group.MapPost("/Logout", Logout);

        return app;
    }

    public static Task<MeResponse> GetMe() => Task.FromResult(new MeResponse("TestUser", true));

    public static async Task Login(HttpContext http)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    public static Task Logout(HttpContext http) => http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}

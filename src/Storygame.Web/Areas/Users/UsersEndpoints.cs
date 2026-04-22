using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi;
using Storygame.Cqrs;
using Storygame.Users;
using Storygame.Users.Commands;
using Storygame.Web.Auth;
using System.Security.Claims;

namespace Storygame.Web.Areas.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization(AuthExtensions.ActionIsRequestedByUserPolicy);

        group.MapGet("/Me", GetMe);
        group.MapPost("/Register", Register).AllowAnonymous();
        group.MapPost("/Login", Login).AllowAnonymous();
        group.MapGet("/ConfirmLogin/{loginConfirmationKey:string}", ConfirmLogin).AllowAnonymous();
        group.MapPost("/Logout", Logout);

        return app;
    }

    public static Task<MeResponse> GetMe() => Task.FromResult(new MeResponse("TestUser", true));

    public static async Task Register(IDispatcher dispatcher, [FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand(request.Name, request.Email);
        await dispatcher.SendAsync(command);
    }

    public static async Task Login([FromBody] LoginRequest request)
    {
        //todo send email with confirmation key
    }

    public static async Task ConfirmLogin(HttpContext http, SessionStorage sessionStorage, [FromRoute] string loginConfirmationKey)
    {
        User user = null!; //todo get user
        var sessionKey = sessionStorage.CreateSession(user, http);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, sessionKey),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    public static Task Logout(HttpContext http) => http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}

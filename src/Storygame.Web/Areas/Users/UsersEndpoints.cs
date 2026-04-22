using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi;
using Storygame.Cqrs;
using Storygame.Users;
using Storygame.Users.Commands;
using Storygame.Users.Queries;
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

    public static async Task Login(IDispatcher dispatcher, HttpContext http, SessionStorage sessionStorage, [FromBody] LoginRequest request)
    {
        var user = (await dispatcher.QueryAsync<GetUserByEmailQuery, GetUserByEmailQueryResult>(new GetUserByEmailQuery(request.Email))).User;
        var confirmationKey = sessionStorage.CreateSession(user, http);
        //todo send email with confirmation key
    }

    public static async Task ConfirmLogin(IDispatcher dispatcher, HttpContext http, SessionStorage sessionStorage, [FromRoute] string loginConfirmationKey)
    {
        var sessionKey = sessionStorage.ConfirmSession(loginConfirmationKey, http);

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

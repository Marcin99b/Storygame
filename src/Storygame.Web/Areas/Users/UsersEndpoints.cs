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
        group.MapPost("/Verify", Verify).AllowAnonymous();
        group.MapPost("/Login", Login).AllowAnonymous();
        group.MapPost("/ConfirmLogin", ConfirmLogin).AllowAnonymous();
        group.MapPost("/Logout", Logout);

        return app;
    }

    public static Task<MeResponse> GetMe() => Task.FromResult(new MeResponse("TestUser", true));

    public static async Task Register(IDispatcher dispatcher, [FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand(request.Name, request.Email);
        await dispatcher.SendAsync(command);
    }

    public static async Task Verify(IDispatcher dispatcher, [FromBody] VerifyUserRequest request)
    {
        var command = new VerifyUserCommand(request.Email, request.VerificationCode);
        await dispatcher.SendAsync(command);
    }

    public static async Task Login(IDispatcher dispatcher, HttpContext http, SessionStorage sessionStorage, [FromBody] LoginRequest request)
    {
        var user = (await dispatcher.QueryAsync<GetUserByEmailQuery, GetUserByEmailQueryResult>(new GetUserByEmailQuery(request.Email))).User;
        if (!user.IsVerified)
        {
            throw new ArgumentException($"User {user.Id} is not verified");
        }

        var confirmationKey = sessionStorage.CreateSession(user, http);
        //todo send email with confirmation key
    }

    public static async Task ConfirmLogin(IDispatcher dispatcher, HttpContext http, SessionStorage sessionStorage, [FromBody] ConfirmLoginRequest request)
    {
        var sessionKey = sessionStorage.ConfirmSession(request.LoginConfirmationKey, http);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, sessionKey),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    public static Task Logout(HttpContext http, UserSessionProvider sessionProvider)
    {
        sessionProvider.Logout();
        return http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}

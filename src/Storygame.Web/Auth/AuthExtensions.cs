using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Storygame.Web.Auth;

public static class AuthExtensions
{
    public const string ActionIsRequestedByUserPolicy = "ActionIsRequestedByUser";

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
    }

    public static void ConfigureAuth(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;

            options.Events.OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = 401;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
        });

        services.AddScoped<UserSession>();

        services.AddAuthorization(options => 
        {
            options.AddPolicy(ActionIsRequestedByUserPolicy, policy => 
            {
                policy.RequireAssertion(policyContext => 
                {
                    var httpContext = policyContext.Resource as HttpContext;
                    if (httpContext == null)
                    {
                        return false;
                    }

                    var session = httpContext.RequestServices.GetRequiredService<UserSession>();
                    if (!session.UserId.HasValue)
                    {
                        return false;
                    }

                    return true;
                });
            });
        });
    }

    public static void ConfigureSetSession(this WebApplication app)
    {
        app.Use((ctx, next) =>
        {
            if (ctx.User.Identity?.IsAuthenticated == true && Guid.TryParse(ctx.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                var session = ctx.RequestServices.GetService<UserSession>();
                if (session != null)
                {
                    session.UserId = userId;
                }
            }

            return next(ctx);
        });
    }
}
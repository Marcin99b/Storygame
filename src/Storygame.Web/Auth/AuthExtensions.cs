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
                    .WithOrigins("http://localhost:5173")
                    .WithMethods("GET", "POST")
                    .WithHeaders("Content-Type", "X-CSRF-TOKEN")
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));
        });
    }

    public static void ConfigureAuth(this IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "__Host-CSRF";
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.HttpOnly = false;
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.Cookie.Name = "__Host-Auth";
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

        services.AddSingleton<SessionStorage>();
        services.AddScoped<UserSessionProvider>();

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

                    var sessionProvider = httpContext.RequestServices.GetRequiredService<UserSessionProvider>();
                    if (string.IsNullOrWhiteSpace(sessionProvider.SessionKey))
                    {
                        return false;
                    }

                    try
                    {
                        var session = sessionProvider.GetSession(httpContext);
                        return session.IsUserVerified && !session.LoggedOut;
                    }
                    catch
                    {
                        //todo log exception
                        return false;
                    }
                });
            });
        });
    }

    public static void ConfigureSetSession(this WebApplication app)
    {
        app.Use((ctx, next) =>
        {
            if (ctx.User.Identity?.IsAuthenticated == true)
            {
                var sessionKey = ctx.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                var sessionProvider = ctx.RequestServices.GetService<UserSessionProvider>();
                if (sessionProvider != null)
                {
                    sessionProvider.SessionKey = sessionKey;
                }
            }

            return next(ctx);
        });
    }
}
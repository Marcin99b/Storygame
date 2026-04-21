using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Storygame.Cqrs;
using Storygame.Logging;
using Storygame.Storage;
using Storygame.Web.Areas.Catalog;
using Storygame.Web.Areas.Library;
using Storygame.Web.Areas.Tracking;
using Storygame.Web.Areas.Users;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
{
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    opts.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddOpenApi();

builder.Services.RegisterCqrs();
builder.Services.RegisterStorage();
builder.Services.RegisterLogging();

builder.Services.AddHealthChecks().AddMongoDb(sp => sp.GetRequiredService<IMongoClient>(), name: "MongoDB");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
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

builder.Services.AddScoped<UserSession>();

builder.Services.AddAuthorization();

var app = builder.Build();

StorageModule.Initialize();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                error = e.Value.Exception?.Message,
            })
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            }));
    }
});

app.UseHttpsRedirection();
app.UseAuthentication();

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

app.UseAuthorization();

app
    .MapCatalogEndpoints()
    .MapLibraryEndpoints()
    .MapTrackingEndpoints()
    .MapUsersEndpoints();

app.MapControllers();

app.Run();


public class UserSession
{
    public Guid? UserId { get; set; }
}
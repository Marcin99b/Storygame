using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using MongoDB.Driver;
using Storygame.Cqrs;
using Storygame.Integrations.Email;
using Storygame.Logging;
using Storygame.Storage;
using Storygame.Web.Areas.Catalog;
using Storygame.Web.Areas.Library;
using Storygame.Web.Areas.Mail;
using Storygame.Web.Areas.Tracking;
using Storygame.Web.Areas.Users;
using Storygame.Web.Auth;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EmailClient>();

builder.Services.AddMemoryCache(options => 
{
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});

builder.Services.ConfigureCors(builder.Configuration);

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
builder.Services.RegisterStorage(builder.Configuration);
builder.Services.RegisterLogging();

builder.Services.AddHealthChecks().AddMongoDb(sp => sp.GetRequiredService<IMongoClient>(), name: "MongoDB");

builder.Services.ConfigureAuth();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; 

    options.AddPolicy("MainRateLimiter", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 3,
                QueueLimit = 0
            }));

    options.AddPolicy("AuthRateLimiter", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 3,
                QueueLimit = 0
            }));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

StorageModule.Initialize();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseCors();

app.UseRateLimiter();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//todo setup cors to hide healthcheck
//it should be available for local network only
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
app.ConfigureSetSession();

app.UseAuthorization();
app.UseAntiforgery();

app
    .MapCatalogEndpoints()
    .MapLibraryEndpoints()
    .MapTrackingEndpoints()
    .MapUsersEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapMailEndpoints();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
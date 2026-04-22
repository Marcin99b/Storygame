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
using Storygame.Web.Auth;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache(options => 
{
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});

builder.Services.ConfigureCors();

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

builder.Services.ConfigureAuth();

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
app.ConfigureSetSession();

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
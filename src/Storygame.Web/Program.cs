using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Storygame.Cqrs;
using Storygame.Storage;
using Storygame.Web.Areas.Catalog;
using Storygame.Web.Areas.Library;
using Storygame.Web.Areas.Tracking;
using Storygame.Web.Areas.Users;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterCqrs();
builder.Services.RegisterStorage();

builder.Services.AddHealthChecks().AddMongoDb(sp => sp.GetRequiredService<IMongoClient>(), name: "MongoDB");

var app = builder.Build();

StorageModule.Initialize();

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

app
    .MapCatalogEndpoints()
    .MapLibraryEndpoints()
    .MapTrackingEndpoints()
    .MapUsersEndpoints();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

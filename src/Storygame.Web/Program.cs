using Storygame.Cqrs;
using Storygame.Web.Areas.Catalog;
using Storygame.Web.Areas.Library;
using Storygame.Web.Areas.Tracking;
using Storygame.Web.Areas.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterCqrs();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/swagger");
}

app
    .MapCatalogEndpoints()
    .MapLibraryEndpoints()
    .MapTrackingEndpoints()
    .MapUsersEndpoints();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

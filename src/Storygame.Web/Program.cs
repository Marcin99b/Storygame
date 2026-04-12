using Storygame.Cqrs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterCqrs();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/swagger");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

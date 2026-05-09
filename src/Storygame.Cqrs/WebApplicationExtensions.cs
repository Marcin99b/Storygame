using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Storygame.Cqrs;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseEventsConsumer(this IApplicationBuilder app, Action<IEventsConsumer> register)
    {
        var service = app.ApplicationServices.GetRequiredService<IEventsConsumer>();
        register(service);
        return app;
    }
}
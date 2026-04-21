using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Storygame.Logging;

public static class LoggingModule
{
    public static void RegisterLogging(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger = logger;

        services.AddSerilog(logger);
    }
}

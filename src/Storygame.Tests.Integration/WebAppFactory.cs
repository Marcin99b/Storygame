using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Storygame.Tests.Integration;

public static class WebAppFactory
{
    public static HttpClient CreateHttpClient(Action<IServiceCollection>? customServices = null)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => 
            {
                builder.ConfigureServices(services => 
                {
                    customServices?.Invoke(services);
                });
            });

        return factory.CreateClient();
    }
}

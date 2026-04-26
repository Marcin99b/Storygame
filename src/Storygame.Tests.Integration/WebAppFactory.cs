using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Storygame.Client;

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

    public static StorygameClient CreateStorygameClient(Action<IServiceCollection>? customServices = null)
        => CreateHttpClient(customServices).ToStorygameClient();

    public static StorygameClient ToStorygameClient(this HttpClient httpClient)
    {
        return new StorygameClient(httpClient);
    }
}

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Storygame.Client;
using Storygame.Storage;
using Storygame.Users;

namespace Storygame.Tests.Integration;

public static class WebAppFactory
{
    public static HttpClient CreateHttpClient(Action<IServiceCollection>? customServices = null)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("environment", "Development");
                builder.ConfigureServices(services =>
                {
                    customServices?.Invoke(services);
                });
            });

        return factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    public static StorygameClient CreateStorygameClient(Action<IServiceCollection>? customServices = null)
        => CreateHttpClient(customServices).ToStorygameClient();

    public static StorygameClient ToStorygameClient(this HttpClient httpClient)
    {
        return new StorygameClient(httpClient);
    }

    public static StorygameClient CreateClientWithDefaultMocks(bool mockDatabase = true)
    {
        return WebAppFactory.CreateStorygameClient(services =>
        {
            if (mockDatabase)
            {
                services.Replace(ServiceDescriptor.Singleton<IUsersRepository, InMemoryUsersRepository>());
                services.Replace(ServiceDescriptor.Singleton<IEventsRepository, InMemoryEventsRepository>());
            }
        });
    }
}

using Microsoft.Net.Http.Headers;
using Storygame.Contracts.WebApi;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Storygame.Client;

public class StorygameClient(Uri address, TimeSpan? customTimeout = null)
{
    private const string CatalogPath = "/api/catalog";
    private const string LibraryPath = "/api/library";
    private const string TrackingPath = "/api/tracking";
    private const string UsersPath = "/api/users";

    private static readonly JsonSerializerOptions jsonOptions = CreateJsonOptions();

    private readonly HttpClient client = new HttpClient()
    {
        BaseAddress = new UriBuilder(address.Scheme, address.Host, address.Port).Uri,
        Timeout = customTimeout ?? TimeSpan.FromSeconds(10),
    };

    public Task Login() 
        => Post(UsersPath + "/Login");

    public Task<MeResponse> Me() 
        => Get<MeResponse>(UsersPath + "/Me");

    public Task<GetCatalogResponse> GetCatalog() 
        => Get<GetCatalogResponse>(CatalogPath);

    public Task AddToLibrary(AddToLibraryRequest request) 
        => Post(LibraryPath, request);

    public Task<GetLibraryResponse> GetLibrary() 
        => Get<GetLibraryResponse>(LibraryPath);

    public Task StartTracking(StartTrackingRequest request) 
        => Post(TrackingPath, request);

    public Task<GetTrackingsResponse> GetTrackings() 
        => Get<GetTrackingsResponse>(TrackingPath);

    public Task UpdateIndex(Guid trackingId, UpdateIndexRequest request) 
        => Post(TrackingPath + $"/{trackingId}/index", request);

    private async Task<TResponse> Get<TResponse>(string url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
        return (await response.Content.ReadFromJsonAsync<TResponse>(jsonOptions))!;
    }

    private async Task Post(string url)
    {
        var response = await client.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
    }

    private async Task Post<TRequest>(string url, TRequest request)
    {
        var response = await client.PostAsJsonAsync(url, request, jsonOptions);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
    }

    private void TryUpdateCookie(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            return;
        }

        var cookie = cookies.Single();

        if (client.DefaultRequestHeaders.Contains(HeaderNames.Cookie))
        {
            client.DefaultRequestHeaders.Remove(HeaderNames.Cookie);
        }

        client.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookie);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.PropertyNameCaseInsensitive = true;

        return options;
    }
}

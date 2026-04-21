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

    public async Task Login()
    {
        var url = UsersPath + "/Login";
        var response = await client.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
    }

    public async Task<MeResponse> Me()
    {
        var url = UsersPath + "/Me";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
        return (await response.Content.ReadFromJsonAsync<MeResponse>(jsonOptions))!;
    }

    public async Task<GetCatalogResponse> GetCatalog()
    {
        var url = CatalogPath;
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
        return (await response.Content.ReadFromJsonAsync<GetCatalogResponse>(jsonOptions))!;
    }

    public async Task AddToLibrary(AddToLibraryRequest request)
    {
        var url = LibraryPath;
        var response = await client.PostAsJsonAsync(url, request, jsonOptions);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
    }

    public async Task<GetLibraryResponse> GetLibrary()
    {
        var url = LibraryPath;
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
        return (await response.Content.ReadFromJsonAsync<GetLibraryResponse>(jsonOptions))!;
    }

    public async Task StartTracking(StartTrackingRequest request)
    {
        var url = TrackingPath;
        var response = await client.PostAsJsonAsync(url, request, jsonOptions);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
    }

    public async Task<GetTrackingsResponse> GetTrackings()
    {
        var url = TrackingPath;
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
        return (await response.Content.ReadFromJsonAsync<GetTrackingsResponse>(jsonOptions))!;
    }

    public async Task UpdateIndex(Guid trackingId, UpdateIndexRequest request)
    {
        var url = TrackingPath + $"/{trackingId}/index";
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

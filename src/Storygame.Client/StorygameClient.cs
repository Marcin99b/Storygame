using Microsoft.Net.Http.Headers;
using Storygame.Contracts.WebApi;
using Storygame.Contracts.WebApi.Requests;
using Storygame.Contracts.WebApi.Responses;
using Storygame.Integrations.Email;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Storygame.Client;

public class StorygameClient
{
    private const string CatalogPath = "/api/catalog";
    private const string LibraryPath = "/api/library";
    private const string TrackingPath = "/api/tracking";
    private const string UsersPath = "/api/users";
    private const string MailPath = "/api/mail";

    private static readonly JsonSerializerOptions jsonOptions = CreateJsonOptions();
    public HttpClient HttpClient { get; }

    public StorygameClient(Uri address, TimeSpan? customTimeout = null)
    {
        HttpClient = new HttpClient()
        {
            BaseAddress = new UriBuilder(address.Scheme, address.Host, address.Port).Uri,
            Timeout = customTimeout ?? TimeSpan.FromSeconds(10),
        };
    }

    public StorygameClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public Task Login(LoginRequest request) 
        => Post(UsersPath + "/Login", request);

    public Task ConfirmLogin(ConfirmLoginRequest request)
        => Post(UsersPath + "/ConfirmLogin", request);

    public Task Register(RegisterRequest request)
        => Post(UsersPath + "/Register", request);

    public Task Verify(VerifyUserRequest request)
        => Post(UsersPath + "/Verify", request);

    public Task<MailMessage[]> Mail(string email)
        => Get<MailMessage[]>(MailPath + $"/{email}");

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
        var response = await HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
        return (await response.Content.ReadFromJsonAsync<TResponse>(jsonOptions))!;
    }

    private async Task Post(string url)
    {
        await UpdateCSRF();
        var response = await HttpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
    }

    private async Task Post<TRequest>(string url, TRequest request)
    {
        await UpdateCSRF();
        var response = await HttpClient.PostAsJsonAsync(url, request, jsonOptions);
        response.EnsureSuccessStatusCode();
        TryUpdateCookie(response);
    }

    private async Task UpdateCSRF()
    {
        var response = await HttpClient.GetAsync(UsersPath + "/CSRF");
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadAsStringAsync();

        if (HttpClient.DefaultRequestHeaders.Contains("X-CSRF-TOKEN"))
        {
            HttpClient.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
        }

        HttpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);
    }

    private void TryUpdateCookie(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            return;
        }

        var cookie = cookies.Single();

        if (HttpClient.DefaultRequestHeaders.Contains(HeaderNames.Cookie))
        {
            HttpClient.DefaultRequestHeaders.Remove(HeaderNames.Cookie);
        }

        HttpClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookie);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.PropertyNameCaseInsensitive = true;

        return options;
    }
}

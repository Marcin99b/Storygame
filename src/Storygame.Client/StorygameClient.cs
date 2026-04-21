using Microsoft.Net.Http.Headers;

namespace Storygame.Client;

public class StorygameClient(Uri address, TimeSpan? customTimeout = null)
{
    private const string CatalogPath = "/api/catalog";
    private const string LibraryPath = "/api/library";
    private const string TrackingPath = "/api/tracking";
    private const string UsersPath = "/api/users";

    private readonly HttpClient client = new HttpClient()
    {
        BaseAddress = new UriBuilder(address.Scheme, address.Host, address.Port).Uri,
        Timeout = customTimeout ?? TimeSpan.FromSeconds(10)
    };

    public async Task Login()
    {
        var url = UsersPath + "/Login";
        var response = await client.PostAsync(url, null);
        var cookie = response.Headers.GetValues("Set-Cookie").Single();

        if (client.DefaultRequestHeaders.Contains(HeaderNames.Cookie))
        {
            client.DefaultRequestHeaders.Remove(HeaderNames.Cookie);
        }
        
        client.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookie);
    }

    public async Task Me()
    {
        var url = UsersPath + "/Me";
        var response = await client.GetAsync(url);
    }
}

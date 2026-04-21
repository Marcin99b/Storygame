namespace Storygame.Client;

public class StorygameClient(Uri address, TimeSpan? customTimeout = null)
{
    private readonly HttpClient client = new HttpClient()
    {
        BaseAddress = new UriBuilder(address.Scheme, address.Host, address.Port).Uri,
        Timeout = customTimeout ?? TimeSpan.FromSeconds(10)
    };

    private static Uri CatalogPath = new Uri("/api/catalog");
    private static Uri LibraryPath = new Uri("/api/library");
    private static Uri TrackingPath = new Uri("/api/tracking");
    private static Uri UsersPath = new Uri("/api/users");

    public async Task Login()
    {
        var response = await client.PostAsync(new Uri(UsersPath, "/Login"), null);

    }
}

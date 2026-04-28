using Microsoft.Extensions.Caching.Memory;

namespace Storygame.Web.Auth;

public class PerEmailThrottle(IMemoryCache cache)
{
    private readonly TimeSpan window = TimeSpan.FromSeconds(15);

    public bool TryAcquire(string scope, string email)
    {
        var key = $"throttle:{scope}:{email.Trim().ToLowerInvariant()}";
        if (cache.TryGetValue(key, out _))
        {
            return false;
        }
        cache.Set(key, true, window);
        return true;
    }
}
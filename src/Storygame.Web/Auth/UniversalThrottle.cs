using Microsoft.Extensions.Caching.Memory;

namespace Storygame.Web.Auth;

public class UniversalThrottle(IMemoryCache cache)
{
    public bool TryAcquire(string scope, string value, int secondsWindow = 15)
    {
        var key = $"throttle:{scope}:{value.Trim().ToLowerInvariant()}";
        if (cache.TryGetValue(key, out _))
        {
            return false;
        }
        cache.Set(key, true, TimeSpan.FromSeconds(secondsWindow));
        return true;
    }
}
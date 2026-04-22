namespace Storygame.Web.Auth;

public static class SessionKeyGenerator
{
    public static string Generate()
        => Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
}
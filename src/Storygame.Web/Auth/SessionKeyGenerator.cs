using System.Security.Cryptography;

namespace Storygame.Web.Auth;

public static class SessionKeyGenerator
{
    public static string Generate()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
}
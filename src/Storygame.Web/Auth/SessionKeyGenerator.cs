using System.Security.Cryptography;

namespace Storygame.Web.Auth;

public static class SessionKeyGenerator
{
    public static string Generate()
        => RandomNumberGenerator.GetHexString(48).ToUpper();
}
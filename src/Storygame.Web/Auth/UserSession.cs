namespace Storygame.Web.Auth;

public class UserSession
{
    private static readonly DateTime MIN_SESSION_CREATED_DATE_TIME = new DateTime(2026, 4, 13);

    public required string SessionKey { get; set; }
    public required bool LoggedOut { get; set; }
    public required Guid UserId { get; set; }
    public required bool IsUserVerified { get; set; }
    public required string IpAddress { get; set; }
    public required string UserAgent { get; set; }
    public required DateTime SessionCreatedAt { get; set; }

    public bool AllowUserToEnterApp(HttpContext context)
    {
        if (LoggedOut)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(SessionKey))
        {
            return false;
        }

        if (UserId == Guid.Empty)
        {
            return false;
        }

        if (IsUserVerified == false)
        {
            return false;
        }

        if (SessionCreatedAt < MIN_SESSION_CREATED_DATE_TIME)
        {
            return false;
        }

        return VerifyHttpContext(context);
    }

    private bool VerifyHttpContext(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent;

        if (IpAddress != ip)
        {
            return false;
        }

        if (UserAgent != userAgent)
        {
            return false;
        }

        return true;
    }
}

namespace Storygame.Web.Auth;

public class UserSession
{
    private static readonly DateTime MIN_SESSION_CREATED_DATE_TIME = new DateTime(2026, 4, 13);

    public required string SessionKey { get; set; }
    public required bool LoggedOut { get; set; }
    public required Guid UserId { get; set; }
    public required bool IsUserVerified { get; set; }
    public required string UserAgent { get; set; }
    public required DateTime SessionCreatedAt { get; set; }
    public DateTime LastApiCall { get; set; }

    public bool AllowUserToEnterApp(HttpContext context, bool isConfirmed)
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

        if (IsExpired(isConfirmed))
        {
            return false;
        }

        return VerifyHttpContext(context);
    }

    public bool IsExpired(bool isConfirmed)
    {
        var now = DateTime.UtcNow;
        if (isConfirmed)
        {
            // api call is set
            if (LastApiCall > MIN_SESSION_CREATED_DATE_TIME)
            {
                return LastApiCall < now.AddDays(-7);
            }
            else
            {
                // user has 15 minutes for first api call after /ConfirmLogin (after ConfirmationKey step)
                return SessionCreatedAt < now.AddMinutes(-15);
            }
        }

        // user has 15 minutes for first api call after /Login (ConfirmationKey step)
        return SessionCreatedAt < now.AddMinutes(-15);
    }

    private bool VerifyHttpContext(HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent;

        if (UserAgent != userAgent)
        {
            return false;
        }

        return true;
    }
}

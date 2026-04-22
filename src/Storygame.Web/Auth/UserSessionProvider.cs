namespace Storygame.Web.Auth;

public class UserSessionProvider(SessionStorage sessionStorage)
{
    public string? SessionKey { get; set; }

    public UserSession GetSession(HttpContext context)
    {
        if (SessionKey == null)
        {
            throw new ArgumentException("Session key is not set");
        }

        var session = sessionStorage.GetSession(SessionKey, context);
        return session;
    }

    public void Logout()
    {
        if (SessionKey == null)
        {
            throw new ArgumentException("Session key is not set");
        }

        sessionStorage.Logout(SessionKey);
    }
}

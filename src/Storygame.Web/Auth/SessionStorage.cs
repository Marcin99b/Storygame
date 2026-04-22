using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Storygame.Cqrs;
using Storygame.Users;
using System.Collections.Concurrent;

namespace Storygame.Web.Auth;

public class SessionStorage
{
    private readonly ConcurrentDictionary<string, UserSession> activeSessions = new ConcurrentDictionary<string, UserSession>();
    //todo scan for long waiting session and delete them
    private readonly ConcurrentDictionary<string, UserSession> notConfirmedSessions = new ConcurrentDictionary<string, UserSession>();

    public string CreateSession(User user, HttpContext context)
    {
        if (!user.IsVerified)
        {
            throw new ArgumentException("User is not verified");
        }

        var confirmationKey = SessionKeyGenerator.Generate();
        var session = new UserSession()
        {
            SessionKey = SessionKeyGenerator.Generate(),
            LoggedOut = false,
            UserId = user.Id,
            IsUserVerified = user.IsVerified,
            IpAddress = context.Connection.RemoteIpAddress!.ToString(),
            UserAgent = context.Request.Headers.UserAgent!,
            SessionCreatedAt = DateTime.UtcNow
        };

        //verify if session is valid for context that created it
        if (session.AllowUserToEnterApp(context) == false)
        {
            throw new Exception($"Session creation is broken.");
        }

        if(notConfirmedSessions.TryAdd(confirmationKey, session) == false)
        {
            throw new Exception($"Cannot save session with key {confirmationKey} for user {user.Id} in memory");
        }

        return confirmationKey;
    }

    public string ConfirmSession(string confirmationKey, HttpContext context)
    {
        if (notConfirmedSessions.Remove(confirmationKey, out var session))
        {
            if (session.AllowUserToEnterApp(context) == false)
            {
                throw new Exception($"Cannot get session for current context");
            }

            var sessionKey = session.SessionKey;
            if (activeSessions.TryAdd(sessionKey, session) == false)
            {
                throw new Exception($"Cannot save session with key {sessionKey} for user {session.UserId} in memory");
            }

            return sessionKey;
        }
        else
        {
            throw new Exception($"Cannot get session for current context");
        }
    }

    public UserSession GetSession(string key, HttpContext context)
    {
        if (activeSessions.TryGetValue(key, out var session))
        {
            if (session.AllowUserToEnterApp(context) == false)
            {
                throw new Exception($"Cannot get session for current context");
            }

            return session;
        }

        throw new ArgumentException($"Cannot get session for key {key}");
    }
}

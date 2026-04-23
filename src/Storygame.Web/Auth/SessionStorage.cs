using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Storygame.Cqrs;
using Storygame.Users;
using System.Collections.Concurrent;

namespace Storygame.Web.Auth;

public class SessionStorage
{
    //todo store sessions in cache database like KeyDB/Redis
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
            UserAgent = context.Request.Headers.UserAgent!,
            SessionCreatedAt = DateTime.UtcNow,
        };

        //verify if session is valid for context that created it
        if (session.AllowUserToEnterApp(context, isConfirmed: false) == false)
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
            if (session.AllowUserToEnterApp(context, isConfirmed: false) == false)
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
            if (session.AllowUserToEnterApp(context, isConfirmed: true) == false)
            {
                activeSessions.Remove(key, out var _);
                throw new Exception($"Cannot get session for current context");
            }

            return session;
        }

        throw new ArgumentException($"Cannot get session for key {key}");
    }

    public void Logout(string key)
    {
        if (activeSessions.ContainsKey(key) == false)
        {
            return;
        }

        if (activeSessions.Remove(key, out var session))
        {
            //change flag in memory
            //block flows executed at now
            session.LoggedOut = true;
        }
        else if (activeSessions.TryGetValue(key, out var sessionGetValue))
        {
            sessionGetValue.LoggedOut = true;
        }
        else
        {
            throw new ArgumentException($"Cannot logout session {key}");
        }
    }
}

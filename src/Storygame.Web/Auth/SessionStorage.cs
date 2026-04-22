using Storygame.Cqrs;
using Storygame.Users;
using System.Collections.Concurrent;

namespace Storygame.Web.Auth;

public class SessionStorage(IDispatcher dispatcher)
{
    private readonly ConcurrentDictionary<string, UserSession> sessions = new ConcurrentDictionary<string, UserSession>();

    public string CreateSession(User user, HttpContext context)
    {
        if (!user.IsVerified)
        {
            throw new ArgumentException("User is not verified");
        }

        var sessionKey = SessionKeyGenerator.Generate();
        var session = new UserSession()
        {
            SessionKey = sessionKey,
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

        if(sessions.TryAdd(sessionKey, session) == false)
        {
            throw new Exception($"Cannot save session with key {sessionKey} for user {user.Id} in memory");
        }

        return sessionKey;
    }

    public UserSession GetSession(string key, HttpContext context)
    {
        if (sessions.TryGetValue(key, out var session))
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
}

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

public static class SessionKeyGenerator
{
    public static string Generate()
        => Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
}
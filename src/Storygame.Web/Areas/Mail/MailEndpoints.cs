
using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi;
using Storygame.Integrations.Email;
using Storygame.Web.Auth;

namespace Storygame.Web.Areas.Mail;

public static class MailEndpoints
{
    /// <summary>
    /// Mail inbox simulator, used to read messages that would be emails in production environment.
    /// It doesn't have auth on purpose.
    /// Mails should be accessible for users without accounts in app, like in real life.
    /// </summary>
    public static IEndpointRouteBuilder MapMailEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/mail").WithTags("Mail")
            .AllowAnonymous()
            .RequireRateLimiting("MainRateLimiter")
            .ValidateAntiforgery();

        group.MapGet("/{email}", ReadMail);

        return app;
    }

    public static async Task<MailMessage[]> ReadMail(EmailClient emailClient, [FromRoute] string email)
    {
        return await emailClient.Read(email);
    }
}
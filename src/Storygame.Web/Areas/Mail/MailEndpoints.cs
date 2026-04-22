
using Microsoft.AspNetCore.Mvc;
using Storygame.Contracts.WebApi;

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
        var group = app.MapGroup("/api/mail").WithTags("Mail").AllowAnonymous();

        group.MapGet("/{email}", ReadMail);

        return app;
    }

    public static async Task<MailMessage[]> ReadMail([FromRoute] string email)
    {
        var messages = new List<MailMessage>() { new MailMessage("test", "abc", DateTime.UtcNow) };

        return messages.ToArray();
    }
}
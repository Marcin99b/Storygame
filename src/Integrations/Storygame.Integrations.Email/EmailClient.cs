using System.Collections.Concurrent;
using System.Net.Mail;

namespace Storygame.Integrations.Email;

public class EmailClient
{
    private readonly ConcurrentBag<MailMessage> messages = new ConcurrentBag<MailMessage>();

    public async Task Send(MailMessage message)
    {
        messages.Add(message);
        await Task.CompletedTask;
    }

    public Task<MailMessage[]> Read(string email)
    {
        var result = messages.Where(x => x.Receiver == email).ToArray();
        return Task.FromResult(result);
    }
}

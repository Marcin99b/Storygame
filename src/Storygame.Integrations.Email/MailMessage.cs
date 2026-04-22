namespace Storygame.Integrations.Email;

public record MailMessage(string Receiver, string Subject, string Message, DateTime SentAt);
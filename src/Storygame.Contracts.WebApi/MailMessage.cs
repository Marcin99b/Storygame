namespace Storygame.Contracts.WebApi;

public record MailMessage(string Subject, string Message, DateTime SentAt);
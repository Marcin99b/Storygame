namespace Storygame.Cqrs;

public record Event
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}

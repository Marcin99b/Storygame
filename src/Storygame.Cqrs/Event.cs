namespace Storygame.Cqrs;

public record Event
{
    public Guid EventId { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}

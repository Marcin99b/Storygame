using Storygame.Exceptions;

namespace Storygame.Ownership;

public class OwnershipException(Guid objectOwnerId, Guid actionPerformer) : StorygameException
{
    public Guid ObjectOwnerId { get; } = objectOwnerId;
    public Guid ActionPerformer { get; } = actionPerformer;

    public override string ToString() => $"User {ActionPerformer} tried to get access to property of user {ObjectOwnerId}";
}
namespace Storygame.Ownership;

public static class OwnerableExtensions
{
    extension(IOwnerable ownerable)
    {
        public bool CheckOwnership(Guid userId) => ownerable.UserId == userId;

        public void ThrowIfNotOwner(Guid userId)
        {
            if (ownerable.CheckOwnership(userId) == false)
            {
                throw new OwnershipException(ownerable.UserId, userId);
            }
        }
    }
}

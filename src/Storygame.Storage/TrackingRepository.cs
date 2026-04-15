using Storygame.Tracking;

namespace Storygame.Storage;

public class TrackingRepository : ITrackingRepository
{
    public void AddTracking(Tracking.Tracking tracking) => throw new NotImplementedException();
    public Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId) => throw new NotImplementedException();
}

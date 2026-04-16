using MongoDB.Driver;
using Storygame.Tracking;

namespace Storygame.Storage;

public class TrackingRepository(IMongoDatabase database) : ITrackingRepository
{
    private readonly IMongoCollection<Tracking.Tracking> tracking = database.GetCollection<Tracking.Tracking>(DbCollectionNames.Tracking);

    public void AddTracking(Tracking.Tracking tracking) => throw new NotImplementedException();
    public Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId) => throw new NotImplementedException();
}

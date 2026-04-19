using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Tracking;

namespace Storygame.Storage;

public class TrackingRepository(IMongoDatabase database) : ITrackingRepository
{
    private readonly IMongoCollection<Tracking.Tracking> tracking = database.GetCollection<Tracking.Tracking>(DbCollectionNames.Tracking);

    public void AddTracking(Tracking.Tracking tracking) => throw new NotImplementedException();
    public Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId) => throw new NotImplementedException();
    public async Task<IEnumerable<Tracking.Tracking>> GetUserTrackings(Guid userId)
    {
        return await tracking.AsQueryable().Where(x => x.UserId == userId).ToListAsync();
    }
}

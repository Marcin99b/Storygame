using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Tracking;

namespace Storygame.Storage;

public class TrackingRepository(IMongoDatabase database) : ITrackingRepository
{
    private readonly IMongoCollection<Tracking.Tracking> trackings = database.GetCollection<Tracking.Tracking>(DbCollectionNames.Tracking);

    public Task AddTracking(Tracking.Tracking tracking) => trackings.InsertOneAsync(tracking);

    public async Task UpdateTracking(Tracking.Tracking tracking) => await trackings.ReplaceOneAsync(x => x.Id == tracking.Id, tracking);

    public async Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId) => (await trackings.CountDocumentsAsync(x => x.LibraryBookId == libraryBookId)) > 0;

    public async Task<IEnumerable<Tracking.Tracking>> GetUserTrackings(Guid userId)
    {
        return await trackings.AsQueryable().Where(x => x.UserId == userId).ToListAsync();
    }
}

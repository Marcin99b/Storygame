using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Tracking;

namespace Storygame.Storage;

public class TrackingRepository(IMongoDatabase database) : ITrackingRepository
{
    private readonly IMongoCollection<Tracking.Tracking> trackings = database.GetCollection<Tracking.Tracking>(DbCollectionNames.TRACKING);

    public Task AddTracking(Tracking.Tracking tracking, CancellationToken ct) => trackings.InsertOneAsync(tracking, null, ct);

    public async Task UpdateTracking(Tracking.Tracking tracking, CancellationToken ct) => await trackings.ReplaceOneAsync(x => x.Id == tracking.Id, tracking, cancellationToken: ct);

    public async Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId, CancellationToken ct) => (await trackings.CountDocumentsAsync(x => x.LibraryBookId == libraryBookId, null, ct)) > 0;

    public async Task<IEnumerable<Tracking.Tracking>> GetUserTrackings(Guid userId, CancellationToken ct)
    {
        return await trackings.AsQueryable().Where(x => x.UserId == userId).ToListAsync(ct);
    }

    public Task<Tracking.Tracking> GetTracking(Guid trackingId, CancellationToken ct) => trackings.AsQueryable().FirstAsync(x => x.Id == trackingId, ct);
}

using DnsClient;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Tracking;

namespace Storygame.Storage;

public class TrackingRepository(IMongoDatabase database) : ITrackingRepository
{
    private readonly IMongoCollection<Tracking.Tracking> trackings = database.GetCollection<Tracking.Tracking>(DbCollectionNames.TRACKING);
    private readonly IMongoCollection<Tracking.TrackingStatistic> trackingsStatistics = database.GetCollection<Tracking.TrackingStatistic>(DbCollectionNames.TRACKING_STATISTICS);

    public Task AddTracking(Tracking.Tracking tracking, CancellationToken ct) => trackings.InsertOneAsync(tracking, null, ct);

    public async Task UpdateTracking(Tracking.Tracking tracking, CancellationToken ct) => await trackings.ReplaceOneAsync(x => x.Id == tracking.Id, tracking, cancellationToken: ct);

    public async Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId, CancellationToken ct) => (await trackings.CountDocumentsAsync(x => x.LibraryBookId == libraryBookId, null, ct)) > 0;

    public async Task<IEnumerable<Tracking.Tracking>> GetUserTrackings(Guid userId, CancellationToken ct)
    {
        return await trackings.AsQueryable().Where(x => x.UserId == userId).ToListAsync(ct);
    }

    public Task<Tracking.Tracking> GetTracking(Guid trackingId, CancellationToken ct) => trackings.AsQueryable().FirstAsync(x => x.Id == trackingId, ct);

    public Task AddStatistic(TrackingStatistic trackingStatistic, CancellationToken ct)
        => trackingsStatistics.InsertOneAsync(trackingStatistic, null, ct);

    public async Task UpdateStatistic(TrackingStatistic trackingStatistic, CancellationToken ct) 
        => await trackingsStatistics.ReplaceOneAsync(x => x.Id == trackingStatistic.Id, trackingStatistic, cancellationToken: ct);

    public async Task<IEnumerable<TrackingStatistic>> GetStatistics(Guid trackingId, TimeRange timeRange, TimePeriod timePeriod, CancellationToken ct)
    {
        return await trackingsStatistics
            .AsQueryable()
            .Where(x => x.TrackingId == trackingId && x.TimePeriod == timePeriod && x.TimeRange.From >= timeRange.From && x.TimeRange.To <= timeRange.To)
            .ToListAsync(ct);
    }

    public async Task<TrackingStatistic?> GetStatisticByTimePoint(Guid trackingId, DateTime timePoint, TimePeriod timePeriod, CancellationToken ct)
    {
        return await trackingsStatistics
            .AsQueryable().FirstOrDefaultAsync(x => x.TrackingId == trackingId && x.TimePeriod == timePeriod && x.TimeRange.From >= timePoint && x.TimeRange.To <= timePoint, ct);
    }
}

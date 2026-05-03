using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Storygame.Cqrs;
using Storygame.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Storage;

public class EventsRepository(IMongoDatabase database)
{
    private readonly IMongoCollection<Event> events = database.GetCollection<Event>(DbCollectionNames.USERS);

    public async Task Publish<T>(T @event, CancellationToken ct)
        where T : Event
    {
        var collection = GetCollection<T>();
        await collection.InsertOneAsync(@event, null, ct);
    }

    public async Task<T> GetById<T>(Guid id, CancellationToken ct)
        where T : Event
    {
        var collection = GetCollection<T>();
        return await collection.AsQueryable().FirstAsync(x => x.EventId == id, ct);
    }

    public async Task<IEnumerable<T>> GetLast<T>(int count, int offset, CancellationToken ct)
        where T : Event
    {
        var collection = GetCollection<T>();
        return await collection.AsQueryable().OrderByDescending(x => x.CreatedAt).Skip(offset).Take(count).ToListAsync(ct);
    }

    private IMongoCollection<T> GetCollection<T>() 
        where T : Event
    {
        return database.GetCollection<T>(DbCollectionNames.EVENTS_PREFIX + nameof(T));
    }
}

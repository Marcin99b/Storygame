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
    private readonly IMongoCollection<IEvent> events = database.GetCollection<IEvent>(DbCollectionNames.USERS);

    public async Task Publish<T>(T @event, CancellationToken ct)
        where T : class, IEvent
    {
        var collection = GetCollection<T>();
        await collection.InsertOneAsync(@event, null, ct);
    }

    public async Task<T> GetById<T>(Guid id, CancellationToken ct)
        where T : class, IEvent
    {
        var collection = GetCollection<T>();
        return await collection.AsQueryable().FirstAsync(x => x.EventId == id, ct);
    }

    private IMongoCollection<T> GetCollection<T>() 
        where T : class, IEvent
    {
        return database.GetCollection<T>(DbCollectionNames.EVENTS_PREFIX + nameof(T));
    }
}

using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Storygame.Library;
using Storygame.Tracking;
using Storygame.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Storage;

public static class StorageModule
{
    public static void RegisterStorage(this IServiceCollection services)
    {
        var mongoSettings = new MongoClientSettings() 
        {
            Server = new MongoServerAddress("localhost", 27017),

            ConnectTimeout = TimeSpan.FromSeconds(10),
            ServerSelectionTimeout = TimeSpan.FromSeconds(10),
            SocketTimeout = TimeSpan.FromSeconds(30),

            MaxConnectionPoolSize = 100,
            MinConnectionPoolSize = 0,
            MaxConnectionIdleTime = TimeSpan.FromMinutes(10),

            RetryWrites = true,
            RetryReads = true,

            ReadConcern = ReadConcern.Majority,
            WriteConcern = WriteConcern.WMajority,
            ReadPreference = ReadPreference.Primary,

            ApplicationName = "Storygame",
        };
        var mongoClient = new MongoClient(mongoSettings);
        var database = mongoClient.GetDatabase("Storygame");

        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton<IMongoDatabase>(database);
        services.AddSingleton<ILibraryRepository, LibraryRepository>();
        services.AddSingleton<ITrackingRepository, TrackingRepository>();
        services.AddSingleton<IUsersRepository, UsersRepository>();
    }

    public static void Initialize()
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String),
        };
        ConventionRegistry.Register("conventions", pack, _ => true);
        BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(typeof(Guid?), new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)));
    }
}

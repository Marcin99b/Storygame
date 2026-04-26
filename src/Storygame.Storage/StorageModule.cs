using Microsoft.Extensions.Configuration;
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
    public static void RegisterStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        var mongoSettings = MongoClientSettings.FromConnectionString(connectionString);

        mongoSettings.ConnectTimeout = TimeSpan.FromSeconds(10);
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
        mongoSettings.SocketTimeout = TimeSpan.FromSeconds(30);
        mongoSettings.MaxConnectionPoolSize = 100;
        mongoSettings.MinConnectionPoolSize = 0;
        mongoSettings.MaxConnectionIdleTime = TimeSpan.FromMinutes(10);
        mongoSettings.RetryWrites = true;
        mongoSettings.RetryReads = true;
        mongoSettings.ReadConcern = ReadConcern.Majority;
        mongoSettings.WriteConcern = WriteConcern.WMajority;
        mongoSettings.ReadPreference = ReadPreference.Primary;
        mongoSettings.ApplicationName = "Storygame";

        var mongoClient = new MongoClient(mongoSettings);
        var database = mongoClient.GetDatabase("Storygame");

        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton<IMongoDatabase>(database);
        services.AddSingleton<ILibraryRepository, LibraryRepository>();
        services.AddSingleton<ITrackingRepository, TrackingRepository>();
        services.AddSingleton<IUsersRepository, UsersRepository>();
    }

    private static bool _initialized = false;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

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

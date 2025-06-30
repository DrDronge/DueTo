using DueTo.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Web;
using DueTo.Domain.Models;

namespace DueTo.Repository;

public class MongoDbContext : IMongoDbContext
{
    public IMongoDatabase Database { get; }

    public MongoDbContext(IConfiguration config, ILogger<MongoDbContext> logger)
    {
        try
        {
            // Configure MongoDB driver settings
            ConfigureMongoDriver();
            
            // Try to get the connection string directly first
            var connectionString = config["MONGO_URI"];
            
            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback to building from individual components
                var dataSource = config["DatabaseConfig:DataSource"];
                var userId = config["DatabaseConfig:UserID"];
                var password = config["DatabaseConfig:Password"];
                var initialCatalog = config["DatabaseConfig:InitialCatalog"];

                if (string.IsNullOrEmpty(dataSource) || string.IsNullOrEmpty(userId) || 
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(initialCatalog))
                {
                    throw new ArgumentException("DatabaseConfig section missing required fields");
                }

                // Build connection string with authSource parameter
                connectionString = $"mongodb://{userId}:{password}@{dataSource}/{initialCatalog}?authSource=admin";
            }

            logger.LogInformation("Attempting to connect to MongoDB with connection string: {ConnectionString}", 
                connectionString.Replace(config["DatabaseConfig:Password"] ?? "", "***"));

            // Configure MongoDB client settings with timeout and retry options
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            
            // Set connection timeout (how long to wait for initial connection)
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);
            
            // Set server selection timeout (how long to wait for server selection)
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
            
            // Set socket timeout (how long to wait for socket operations)
            settings.SocketTimeout = TimeSpan.FromSeconds(30);
            
            // Set max connection pool size
            settings.MaxConnectionPoolSize = 100;
            
            // Set min connection pool size
            settings.MinConnectionPoolSize = 5;
            
            // Enable retry writes
            settings.RetryWrites = true;
            
            // Set heartbeat timeout
            settings.HeartbeatTimeout = TimeSpan.FromSeconds(10);
            
            // Set max idle time
            settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(10);
            
            // Set max connection lifetime
            settings.MaxConnectionLifeTime = TimeSpan.FromMinutes(30);

            var client = new MongoClient(settings);
            
            // Extract database name from connection string
            var databaseName = GetDatabaseNameFromConnectionString(connectionString);
            Database = client.GetDatabase(databaseName);
            
            // Test the connection
            var pingResult = Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Result;
            logger.LogInformation("Successfully connected to MongoDB database: {DatabaseName}", databaseName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to MongoDB");
            throw;
        }
    }

    private static void ConfigureMongoDriver()
    {
        // Register TaskModel class mapping if not already registered
        if (!BsonClassMap.IsClassMapRegistered(typeof(TaskModel)))
        {
            BsonClassMap.RegisterClassMap<TaskModel>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id);
                cm.IdMemberMap.SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
            });
        }
    }

    private string GetDatabaseNameFromConnectionString(string connectionString)
    {
        try
        {
            var uri = new Uri(connectionString);
            var databaseName = uri.AbsolutePath.TrimStart('/');
            
            // If no database specified in path, try to get from query string or use default
            if (string.IsNullOrEmpty(databaseName))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                databaseName = query["database"] ?? "admin";
            }
            
            return databaseName;
        }
        catch
        {
            // Fallback: try to extract from the end of the connection string
            var parts = connectionString.Split('/');
            if (parts.Length > 0)
            {
                var lastPart = parts[^1];
                // Remove any query parameters
                var databaseName = lastPart.Split('?')[0];
                return string.IsNullOrEmpty(databaseName) ? "admin" : databaseName;
            }
            
            return "admin";
        }
    }
}
using DueTo.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DueTo.Repository;

public class MongoDbContext : IMongoDbContext
{
    public IMongoDatabase Database { get; }

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MONGO_URI"] ?? 
                               throw new ArgumentNullException("MONGO_URI", "Missing MongoDB connection string.");
        
        var mongoUrl = MongoUrl.Create(connectionString);
        var client = new MongoClient(mongoUrl);
        Database = client.GetDatabase(mongoUrl.DatabaseName ?? "DueTo");
    }
    
}
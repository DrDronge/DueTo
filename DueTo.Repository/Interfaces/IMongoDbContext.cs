using MongoDB.Driver;

namespace DueTo.Repository.Interfaces;

public class IMongoDbContext
{
    public IMongoDatabase Database { get; }
}
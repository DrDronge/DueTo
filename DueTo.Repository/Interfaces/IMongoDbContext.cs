using MongoDB.Driver;

namespace DueTo.Repository.Interfaces;

public interface IMongoDbContext
{
    public IMongoDatabase Database { get; }
}
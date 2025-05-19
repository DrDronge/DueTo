using DueTo.Repository.Interfaces;
using MongoDB.Driver;

namespace DueTo.Repository;

public class TaskRepository
{
    private readonly IMongoCollection<Task> _tasks;

    public TaskRepository(IMongoDbContext context)
    {
        _tasks = context.Database.GetCollection<Task>("Tasks");
    }

    public async Task<IEnumerable<Task>> GetAllAsync()
    {
        return await _tasks.Find(task => true).ToListAsync();
    }

    public async Task InsertAsync(Task task)
    {
        await _tasks.InsertOneAsync(task);
    }
}
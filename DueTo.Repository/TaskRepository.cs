using DueTo.Domain.Models;
using DueTo.Repository.Interfaces;
using MongoDB.Driver;

namespace DueTo.Repository;

public class TaskRepository : ITaskRepository
{
    private readonly IMongoCollection<TaskModel>? _tasks;

    public TaskRepository(IMongoDbContext context)
    {
        _tasks = context.Database.GetCollection<TaskModel>("Tasks");
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        return await _tasks.Find(task => true).ToListAsync();
    }

    public async Task InsertAsync(TaskModel task)
    {
        await _tasks!.InsertOneAsync(task);
    }

    public async Task<TaskModel?> GetTaskById(Guid id)
    {
        return await _tasks.Find(task => task.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateTask(TaskModel task)
    {
       await _tasks.ReplaceOneAsync(x => x.Id == task.Id, task);
    }

    public async Task DeleteTask(Guid id)
    {
        await _tasks.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<TaskModel>> GetTasksByDay(Day dayOfWeek)
    {
        return await _tasks.Find(task => task.ActiveDays != null && task.ActiveDays.Contains(dayOfWeek)).ToListAsync();
    }
}
using System.Net;
using DueTo.Domain.Models;
using DueTo.Repository;

namespace DueTo.Service;

public class TaskService(TaskRepository repository)
{
    public async Task<IEnumerable<Task>> GetAllAsync()
    {
        var tasks = await repository.GetAllAsync();
        return tasks;
    }
}
using DueTo.Domain.Models;
using MongoDB.Driver;

namespace DueTo.Repository.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task InsertAsync(TaskModel task);
    Task<TaskModel?> GetTaskById(Guid id);
    Task UpdateTask(TaskModel task);
    Task DeleteTask(Guid id);
    Task<IEnumerable<TaskModel>> GetTasksByDay(Day dayOfWeek);
}
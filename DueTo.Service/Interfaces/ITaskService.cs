using DueTo.Domain.Models;

namespace DueTo.Service.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync();
    Task<TaskDto?> GetTaskById(Guid id);
    Task<TaskDto> CreateTask(TaskDto task);
    Task<TaskDto?> UpdateTask(TaskDto task);
    Task DeleteTask(Guid id);
    Task<IEnumerable<TaskDto>> GetTaskByDay(Day dayOfWeek);
}
using System.Net;
using DueTo.Domain.Models;
using DueTo.Repository.Interfaces;
using DueTo.Service.Interfaces;

namespace DueTo.Service;

public class TaskService(ITaskRepository repository) : ITaskService
{
    public async Task<IEnumerable<TaskDto>> GetAllAsync()
    {
        var tasks = await repository.GetAllAsync();
        return tasks.Select(ToTaskDto);
    }

    public async Task<TaskDto?> GetTaskById(Guid id)
    {
        var task = await repository.GetTaskById(id);
        return task != null ? ToTaskDto(task) : null;
    }

    public async Task<TaskDto> CreateTask(TaskDto taskDto)
    {
        var task = ToTaskModel(taskDto);
        await repository.InsertAsync(task);
        return ToTaskDto(task);
    }

    public async Task<TaskDto?> UpdateTask(TaskDto taskDto)
    {
        if (string.IsNullOrEmpty(taskDto.Id) || !Guid.TryParse(taskDto.Id, out var id))
        {
            throw new ArgumentException("Invalid task ID format");
        }
        
        var task = new TaskModel
        {
            Id = id,
            Text = taskDto.Text,
            Color = taskDto.Color,
            Type = taskDto.Type,
            Priority = taskDto.Priority,
            IsDone = taskDto.IsDone,
            ActiveDays = taskDto.ActiveDays
        };
        
        await repository.UpdateTask(task);
        return ToTaskDto(task);
    }

    public async Task DeleteTask(Guid id)
    {
        await repository.DeleteTask(id);
    }

    public async Task<IEnumerable<TaskDto>> GetTaskByDay(Day dayOfWeek)
    {
        var tasks = await repository.GetTasksByDay(dayOfWeek);
        return tasks.Select(ToTaskDto);
    }

    private static TaskDto ToTaskDto(TaskModel task) => new()
    {
        Id = task.Id.ToString(),
        Text = task.Text,
        Color = task.Color,
        Type = task.Type,
        Priority = task.Priority,
        IsDone = task.IsDone,
        ActiveDays = task.ActiveDays
    };

    private static TaskModel ToTaskModel(TaskDto dto) => new()
    {
        Text = dto.Text,
        Color = dto.Color,
        Type = dto.Type,
        Priority = dto.Priority,
        IsDone = dto.IsDone,
        ActiveDays = dto.ActiveDays
    };
}
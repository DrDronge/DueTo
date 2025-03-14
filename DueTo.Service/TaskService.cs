﻿using System.Net;
using DueTo.Domain.Models;
using DueTo.Repository;

namespace DueTo.Service;

public class TaskService(TaskRepository repository)
{
    public async Task<List<TaskModel>> GetTaskByDay(string dayOfWeek)
    {
        return await repository.GetTasksByDay(dayOfWeek);
    }

    public async Task<List<TaskModel>> GetAllTasks()
    {
        return await repository.GetAllTasksAsync();
    }

    public async Task<TaskModel> GetTaskById(string id)
    {
        return await repository.GetTaskById(id);
    }

    public TaskModel CreateTask(TaskModel task)
    {
        return repository.CreateTask(task);
    }

    public Task<TaskModel> UpdateTaskById(TaskModel task)
    {
        return repository.UpdateTaskById(task);
    }
}
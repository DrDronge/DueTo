using System.Net;
using DueTo.Domain.Models;
using Microsoft.Data.SqlClient;

namespace DueTo.Repository;

public class TaskRepository
{
    List<TaskModel> tasks = new List<TaskModel>
    {
        new()
        {
            Text = "Task 1: Design the homepage",
            Color = "Blue",
            Type = "Design",
            Priority = "High",
            IsDone = false,
            ActiveDays = new List<Day> { Day.Friday, Day.Tuesday, Day.Wednesday }
        },
        new()
        {
            Text = "Task 2: Write API endpoints",
            Color = "Green",
            Type = "Development",
            Priority = "Medium",
            IsDone = true,
            ActiveDays = new List<Day> { Day.Friday, Day.Saturday, Day.Sunday }
        },
        new()
        {
            Text = "Task 3: Prepare presentation",
            Color = "Yellow",
            Type = "Management",
            Priority = "Low",
            IsDone = false,
            ActiveDays = new List<Day> { Day.Monday }
        }
    };

    public Task<List<TaskModel>> GetTasksByDay(string dayOfWeek)
    {
        return Task.FromResult(tasks.Where(t => t.ActiveDays.Contains(Enum.Parse<Day>(dayOfWeek, true))).ToList());
    }

    public Task<List<TaskModel>> GetAllTasks()
    {
        using var connection = Connection.GetConnection();
        string query = "SELECT * FROM Tasks";
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        List<TaskModel> tasks = new List<TaskModel>();
        
        while (reader.Read())
        {
            var task = new TaskModel
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                Text = reader.GetString(reader.GetOrdinal("Text")),
                Type = reader.GetString(reader.GetOrdinal("Type")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                IsDone = reader.GetBoolean(reader.GetOrdinal("IsDone")),
                ActiveDays = new List<Day>(reader.GetInt32(reader.GetOrdinal("ActiveDays")))
            };
            tasks.Add(task);
        };
        connection.Close();
        reader.Close();
        return Task.FromResult(tasks);
    }

    public Task<TaskModel> GetTaskById(string id)
    {
        return Task.FromResult(tasks.First(t => t.Id == id));
    }

    public HttpStatusCode CreateTask(TaskModel task)
    {
        tasks.Add(task);
        return HttpStatusCode.Created;
    }

    public TaskModel UpdateTaskById(TaskModel task)
    {
        var taskToUpdate = tasks.FirstOrDefault(t => t.Id == task.Id);

        if (taskToUpdate == null)
            throw new KeyNotFoundException();

        taskToUpdate.Text = task.Text;
        taskToUpdate.Color = task.Color;
        taskToUpdate.Type = task.Type;
        taskToUpdate.Priority = task.Priority;
        taskToUpdate.IsDone = task.IsDone;
        taskToUpdate.ActiveDays = task.ActiveDays;
    
        return taskToUpdate;
    }
}
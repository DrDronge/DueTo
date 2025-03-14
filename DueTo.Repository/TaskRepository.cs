using System.Net;
using Dapper;
using DueTo.Domain.Models;
using Microsoft.Data.SqlClient;

namespace DueTo.Repository;

public class TaskRepository
{

    public async Task<List<TaskModel>> GetTasksByDay(string dayOfWeek)
    {
        using var connection = Connection.GetConnection();
        
        var sql = "SELECT * FROM Tasks WHERE Day = @day";
        
        var tasks = await connection.QueryAsync<TaskModel>(sql, new { day = dayOfWeek });

        return tasks.ToList();
    }

    public async Task<List<TaskModel>> GetAllTasksAsync()
    {
        using var connection = Connection.GetConnection();
        
        var query = @"SELECT t.*, d.Day FROM Tasks t 
                         INNER JOIN TaskDays td ON t.Id = td.TaskId 
                         INNER JOIN Days d ON td.DayID = d.Id";
        
        var tasks = await connection.QueryAsync<TaskModel>(query);

        return tasks.ToList();
    }

    public Task<TaskModel> GetTaskById(string id)
    {
        using var connection = Connection.GetConnection();
        
        var sql = @"SELECT t.*, d.Day FROM Tasks t 
                       INNER JOIN TaskDays td ON t.TaskId = td.TaskId 
                       INNER JOIN Days d ON td.DayId = d.DayId 
                    WHERE t.Id";
        
        return connection.QuerySingleOrDefaultAsync<TaskModel>(sql, new { Id = id });
    }

    public TaskModel CreateTask(TaskModel task)
    {
        using var connection = Connection.GetConnection();
        
        const string sql = @"INSERT INTO Tasks (Id, Text, Color, Type, Priority, IsDone, ActiveDays) 
                                OUTPUT INSERTED.* 
                                VALUES (@Id, @Text, @Color, @Type, @Priority, @IsDone, @ActiveDays)";
        
        var id = Guid.NewGuid();
        
        var parameters = new
        {
            Id = id,
            Text = task.Text,
            Color = task.Color,
            Type = task.Type,
            Priority = task.Priority,
            IsDone = task.IsDone,
            ActiveDays = task.ActiveDays
        };
        
        var result = connection.QuerySingle<TaskModel>(sql, parameters);
        
        var taskDays = InsertIntoTaskDays(id, task.ActiveDays);
        
        if (result is null)
            throw new Exception("Task can't be created");
        
        return result;
    }

    public async Task<TaskModel> UpdateTaskById(TaskModel task)
    {
        var connection = Connection.GetConnection();
        
        var sql = @"UPDATE Tasks
                     SET Text = @Text,
                         Color = @Color,
                         Type = @Type,
                         Priority = @Priority,
                         IsDone = @IsDone,
                     WHERE Id = @Id";

        var parameters = new
        {
            Id = task.Id,
            Text = task.Text,
            Color = task.Color,
            Type = task.Type,
            Priority = task.Priority,
            IsDone = task.IsDone,
        };
        
        var result = await connection.ExecuteScalarAsync<TaskModel>(sql, parameters);
        
        var taskDays = InsertIntoTaskDays(result.Id, task.ActiveDays);
        
        result.ActiveDays = ConvertToDay();
        
        if (result is null)
             throw new Exception("No task with given Id was found");
        
        return task;
    }

    private List<Day>? ConvertToDay()
    {
        throw new NotImplementedException();
    }

    public List<TaskDayModel> InsertIntoTaskDays(Guid taskId, List<Day> days)
    {
        
        var daysGuids = GetGuidFromDays(days);
        
        using var connection = Connection.GetConnection();
        
        var sql = @"INSERT INTO TaskDays (TaskId, Day) OUTPUT INSERTED.* VALUES (@TaskId, @Day)";
        
        var parameter = daysGuids.Select(x => new {TaskId = taskId, Day = x}).ToList();
        
        return connection.Query<TaskDayModel>(sql, parameter).ToList();
        
    }

    private List<Guid> GetGuidFromDays(List<Day> days)
    {
        var connection = Connection.GetConnection();
        
        var sql = "SELECT Id FROM Days WHERE Day IN @Day";

        var parameters = new
        {
            Day = days
        };
        
        return connection.Query<Guid>(sql, parameters).ToList();
    }
}
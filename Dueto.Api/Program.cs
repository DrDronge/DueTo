using System.Net;
using System.Text.Json;
using Dueto.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.HttpStatusCode;
using static Microsoft.AspNetCore.Http.Results;
using Task = Dueto.Api.Models.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var tasks = new List<Task>
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
        ActiveDays = new List<Day> {Day.Friday, Day.Saturday, Day.Sunday}
    },
    new()
    {
        Text = "Task 3: Prepare presentation",
        Color = "Yellow",
        Type = "Management",
        Priority = "Low",
        IsDone = false,
        ActiveDays = new List<Day> {Day.Monday}
    }
};

app.MapGet("/task", (string DayOfWeek) =>
    {
        return tasks.Where(t => t.ActiveDays.Contains(Enum.Parse<Day>(DayOfWeek, true))).ToList();
    })
    .WithName("GetTasksByDay");

app.MapGet("/allTasks", () => tasks).WithName("GetAllTasks");

app.MapGet("/taskbyid", (string Id) =>
{
    return tasks.Where(t => t.Id == Id);
}).WithName("GetTaskById");

app.MapPost("/task", async (Task task) =>
{
    tasks.Add(task);
    return HttpStatusCode.Created;
}).WithName("CreateTask");

app.MapPut("/task", (Task task) =>
{
    var taskToUpdate = tasks.FirstOrDefault(t => t.Id == task.Id);

    if (taskToUpdate == null)
        return NotFound();

    taskToUpdate.Text = task.Text;
    taskToUpdate.Color = task.Color;
    taskToUpdate.Type = task.Type;
    taskToUpdate.Priority = task.Priority;
    taskToUpdate.IsDone = task.IsDone;
    taskToUpdate.ActiveDays = task.ActiveDays;
    
    return Ok(taskToUpdate);
}).WithName("UpdateTask");
app.Run();
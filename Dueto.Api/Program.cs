using Dueto.Api.Models;
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

app.Run();
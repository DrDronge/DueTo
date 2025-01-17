using System.Net;
using System.Text.Json;
using DueTo.Domain.Models;
using DueTo.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.HttpStatusCode;
using static Microsoft.AspNetCore.Http.Results;
using DueTo.Service;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddSingleton<Connection>();

Connection.SetConfiguration(builder.Configuration);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("DueTo")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();



app.MapGet("/task", (string dayOfWeek, TaskService taskService) => Ok(taskService.GetTaskByDay(dayOfWeek)))
    .WithName("GetTasksByDay");

app.MapGet("/allTasks", (TaskService taskService) => taskService.GetAllTasks()).WithName("GetAllTasks");

app.MapGet("/taskbyid", (string id, TaskService taskService) => taskService.GetTaskById(id)).WithName("GetTaskById");

app.MapPost("/task", (TaskModel task, TaskService taskService) => taskService.CreateTask(task)).WithName("CreateTask");

app.MapPut("/task", (TaskModel task, TaskService taskService) => taskService.UpdateTaskById(task)).WithName("UpdateTask");
app.Run();
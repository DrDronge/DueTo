using System.Net;
using System.Text.Json;
using DueTo.Domain.Models;
using DueTo.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.HttpStatusCode;
using static Microsoft.AspNetCore.Http.Results;
using DueTo.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskRepository>();
builder.Services.AddScoped<TaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapGet("/task", (string dayOfWeek, TaskService taskService) => Ok(taskService.GetTaskByDay(dayOfWeek)))
    .WithName("GetTasksByDay");

app.MapGet("/allTasks", (TaskService taskService) => taskService.GetAllTasks()).WithName("GetAllTasks");

app.MapGet("/taskbyid", (string id, TaskService taskService) => taskService.GetTaskById(id)).WithName("GetTaskById");

app.MapPost("/task", (TaskModel task, TaskService taskService) => taskService.CreateTask(task)).WithName("CreateTask");

app.MapPut("/task", (TaskModel task, TaskService taskService) => taskService.UpdateTaskById(task)).WithName("UpdateTask");
app.Run();
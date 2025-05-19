using System.Net;
using System.Text.Json;
using DueTo.Domain.Models;
using DueTo.Repository;
using DueTo.Repository.Interfaces;
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
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

Connection.SetConfiguration(builder.Configuration);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowScalarOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:8080")  // Allow Scalar's origin
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
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
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .AddServer("http://localhost:8080");
    });
}

// app.UseHttpsRedirection();
app.UseCors("AllowScalarOrigin");



app.MapGet("/task", (string dayOfWeek, TaskService taskService) => Ok(taskService.GetTaskByDay(dayOfWeek)))
    .WithName("GetTasksByDay");

app.MapGet("/allTasks", (TaskService taskService) => taskService.GetAllAsync()).WithName("GetAllTasks");

app.MapGet("/taskbyid", (string id, TaskService taskService) => taskService.GetTaskById(id)).WithName("GetTaskById");

app.MapPost("/task", (TaskModel task, TaskService taskService) => taskService.CreateTask(task)).WithName("CreateTask");

app.MapPut("/task", (TaskModel task, TaskService taskService) => taskService.UpdateTaskById(task)).WithName("UpdateTask");
app.Run();
using System.Text.Json;
using System.Text.Json.Serialization;
using DueTo.Domain.Models;
using DueTo.Repository;
using DueTo.Repository.Interfaces;
using DueTo.Service;
using DueTo.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Configure JSON serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new GuidJsonConverter());
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<ITaskRepository, TaskRepository>();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

builder.WebHost.ConfigureKestrel(serverOptions => { }) // Optional customization
    .UseUrls(builder.Configuration["Urls"]!);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowScalarOrigin", policy =>
    {
        policy.AllowAnyOrigin() // Allow Scalar's origin
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
            .AddServer("http://host.docker.internal:8080");
    });
}

// app.UseHttpsRedirection();
app.UseCors("AllowScalarOrigin");

// API Endpoints
app.MapGet("/taskbyday", (Day dayOfWeek, ITaskService taskService) => 
    taskService.GetTaskByDay(dayOfWeek))
    .WithName("GetTasksByDay");

app.MapGet("/allTasks", (ITaskService taskService) => 
    taskService.GetAllAsync())
    .WithName("GetAllTasks");

app.MapGet("/taskbyid", (Guid id, ITaskService taskService) => 
    taskService.GetTaskById(id))
    .WithName("GetTaskById");

app.MapPost("/task", (TaskDto taskDto, ITaskService taskService) => 
    taskService.CreateTask(taskDto))
    .WithName("CreateTask");

app.MapPut("/task", (TaskDto taskDto, ITaskService taskService) => 
    taskService.UpdateTask(taskDto))
    .WithName("UpdateTask");

app.MapDelete("/delete", (Guid id, ITaskService taskService) => 
    taskService.DeleteTask(id))
    .WithName("DeleteTask");

app.MapGet("/health", (IMongoDbContext mongoContext) => 
{
    try
    {
        // Test the connection
        var pingResult = mongoContext.Database.RunCommandAsync((MongoDB.Driver.Command<MongoDB.Bson.BsonDocument>)"{ping:1}").Result;
        return Results.Ok(new { status = "healthy", database = "connected" });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
}).WithName("HealthCheck");

app.Run();

// Custom JSON converter for GUIDs
public class GuidJsonConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (Guid.TryParse(stringValue, out var guid))
            {
                return guid;
            }
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return Guid.Empty;
        }
        
        throw new JsonException($"Cannot convert {reader.TokenType} to Guid");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
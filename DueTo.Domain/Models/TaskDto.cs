using DueTo.Domain.Models;
using System.Text.Json.Serialization;

namespace Dueto.Api.Models;

public class TaskDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("priority")]
    public string? Priority { get; set; }
    
    [JsonPropertyName("isDone")]
    public bool? IsDone { get; set; }
    
    [JsonPropertyName("activeDays")]
    public List<Day>? ActiveDays { get; set; }
}
namespace DueTo.Domain.Models;

public class TaskModel
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public string? Color { get; set; }
    public string? Type { get; set; }
    public string? Priority { get; set; }
    public bool? IsDone { get; set; }
    public List<Day>? ActiveDays { get; set; }
}
namespace DueTo.Domain.Models;

public class TaskModel
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public string Color { get; set; }
    public string Type { get; set; }
    public string Priority { get; set; }
    public bool IsDone { get; set; }
    public List<Day>? ActiveDays { get; set; }
    
    public TaskModel(Guid id, string text, string color, string type, string priority, bool isDone, List<Day> activeDays)
    {
        Id = id;
        Text = text;
        Color = color;
        Type = type;
        Priority = priority;
        IsDone = isDone;
        ActiveDays = activeDays;
    }

    public TaskModel(Guid id, string text, string color, string type, string priority, bool isDone)
    {
        Id = id;
        Text = text;
        Color = color;
        Type = type;
        Priority = priority;
        IsDone = isDone;
    }
}
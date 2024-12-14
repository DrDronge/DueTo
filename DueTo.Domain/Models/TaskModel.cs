namespace DueTo.Domain.Models;

public class TaskModel
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Color { get; set; }
    public string Type { get; set; }
    public string Priority { get; set; }
    public bool IsDone { get; set; }
    public List<Day> ActiveDays { get; set; }
    
    public TaskModel(string text, string color, string type, string priority, bool isDone, List<Day> activeDays)
    {
        Id = Guid.NewGuid().ToString();
        Text = text;
        Color = color;
        Type = type;
        Priority = priority;
        IsDone = isDone;
        ActiveDays = activeDays;
    }

    public TaskModel()
    {
        Id ??= Guid.NewGuid().ToString();
    }
}
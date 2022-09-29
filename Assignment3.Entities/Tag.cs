namespace Assignment3.Entities;

public class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<WorkItem> Tasks { get; set; } = new List<WorkItem>();
}

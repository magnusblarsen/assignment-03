namespace Assignment3.Entities;

public class Tag
{
    public int Id { get; set; }

    #pragma warning disable
    public string Name { get; set; }

    public List<WorkItem> Tasks { get; set; }
}

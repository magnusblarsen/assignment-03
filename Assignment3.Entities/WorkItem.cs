namespace Assignment3.Entities;

public class WorkItem
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; }

    public User AssignedTo { get; set; } = null!;

    public string Description { get; set; } = null!;

    public State State { get; set; }

    public List<Tag> Tags { get; set; }
}

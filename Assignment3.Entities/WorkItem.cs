namespace Assignment3.Entities;

public class WorkItem
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public User? AssignedTo { get; set; }

    public string? Description { get; set; }

    public State State { get; set; }

    public List<Tag> Tags { get; set; } = new List<Tag>();

    public DateTime Created {get; set; }

    public DateTime StateUpdated {get; set; }
}

namespace Assignment3.Entities;
using static Assignment3.Core.Response;
using static Assignment3.Core.State;

public class TaskRepository : ITaskRepository
{
    private KanbanContext _context;

    public TaskRepository(KanbanContext context) => _context = context;

    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {
        // Find user
        var assignedTo = _context.Users.Find(task.AssignedToId);

        if (assignedTo is null) 
        {
            return (BadRequest, -1);
        }

        // Find tags
        var tags = new List<Tag>();
        foreach (var tagName in task.Tags)
        {
            var tag = _context.Tags.Where(t => t.Name == tagName).FirstOrDefault();
            if (tag is null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
            }
            tags.Add(tag);
        }

        // Create task entity
        var entity = new WorkItem { Title = task.Title, AssignedTo = assignedTo, Description = task.Description, State = New, Tags = tags, Created = DateTime.Now, StateUpdated = DateTime.Now };

        // Add task to user
        assignedTo.Tasks.Add(entity);
        
        // Add task to tags
        tags.ForEach(t => t.Tasks.Add(entity));

        _context.Tasks.Add(entity);
        _context.SaveChanges();
        return (Created, entity.Id);
    }

    public Response Delete(int taskId)
    {
        var entity = _context.Tasks.Find(taskId);
        if (entity is null)
        {
            return NotFound;
        }

        switch (entity.State)
        {
            case New:
                _context.Tasks.Remove(entity);
                break;
            case Active:
                entity.State = Removed;
                break;
            default:
                return Conflict;
        }

        _context.SaveChanges();
        return Deleted;
    }

    public TaskDetailsDTO? Read(int taskId)
    {
        var entity = _context.Tasks.Find(taskId);
        if (entity is null)
        {
            return null;
        }
        return new TaskDetailsDTO(entity.Id, entity.Title, entity.Description ?? string.Empty, entity.Created, entity.AssignedTo?.Name ?? string.Empty, entity.Tags.Select(t => t.Name).ToList().AsReadOnly(), entity.State, entity.StateUpdated);
    }

    public IReadOnlyCollection<TaskDTO> ReadAll() => _context.Tasks.Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo == null ? string.Empty : t.AssignedTo.Name, t.Tags.Select(t => t.Name).ToList().AsReadOnly(), t.State)).ToList().AsReadOnly();

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state) => _context.Tasks.Where(t => t.State == state).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo == null ? string.Empty : (t.AssignedTo.Name), t.Tags.Select(t => t.Name).ToList().AsReadOnly(), t.State)).ToList().AsReadOnly();

    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag) => _context.Tasks.Where(t => t.Tags.Any(taskTag => taskTag.Name == tag)).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo == null ? string.Empty : (t.AssignedTo.Name), t.Tags.Select(t => t.Name).ToList().AsReadOnly(), t.State)).ToList().AsReadOnly();

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId) => _context.Tasks.Where(t => t.AssignedTo == null ? false : t.AssignedTo.Id == userId).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo == null ? string.Empty : (t.AssignedTo.Name), t.Tags.Select(t => t.Name).ToList().AsReadOnly(), t.State)).ToList().AsReadOnly();

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved() => _context.Tasks.Where(t => t.State == Removed).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo == null ? string.Empty : (t.AssignedTo.Name), t.Tags.Select(t => t.Name).ToList().AsReadOnly(), t.State)).ToList().AsReadOnly();

    public Response Update(TaskUpdateDTO task)
    {
        var entity = _context.Tasks.Find(task.Id);
        if (entity is null)
        {
            return NotFound;
        }

        // Find user
        var assignedTo = _context.Users.Find(task.AssignedToId);
        if (assignedTo is null) 
        {
            return BadRequest;
        }

        // Find tags
        var tags = new List<Tag>();
        foreach (var tagName in task.Tags)
        {
            var tag = _context.Tags.Find(tagName);
            if (tag is null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
            }
            tags.Add(tag);
        }

        // Unassign previous user
        var oldAssignee = entity.AssignedTo;
        oldAssignee.Tasks.Remove(entity);

        // Unassign old tags
        var oldTags = entity.Tags;
        oldTags.ForEach( t => t.Tasks.Remove(entity) );

        // Update properties
        entity.Title = task.Title;
        entity.Description = task.Description;
        entity.AssignedTo = assignedTo;
        entity.Tags = tags;
        entity.State = task.State;
        entity.StateUpdated = DateTime.Now;
        
        // Add task to user
        assignedTo.Tasks.Add(entity);
        
        // Add task to tags
        tags.ForEach(t => t.Tasks.Add(entity));

        return Updated;
    }
}
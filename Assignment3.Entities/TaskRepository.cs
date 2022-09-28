namespace Assignment3.Entities;
using static Assignment3.Core.Response;
using static Assignment3.Core.State;

public class TaskRepository : ITaskRepository
{
    private KanbanContext _context;

    public TaskRepository(KanbanContext context) => _context = context;

    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {

        WorkItem? entity = _context.Tasks.FirstOrDefault(t => t.Title == task.Title);
        Response response;

        if (entity is not null)
        {
            response = Conflict;
        } else 
        {
            var assignedTo = _context.Users.Find(task.AssignedToId); 
            if (assignedTo is null)
            {
                response = Conflict;
                return (response, entity.Id);
            }
            var tags = 
            //TODO: StateUpdated to current time
            entity = new WorkItem{Title = task.Title, AssignedTo = assignedTo, Description = task.Description, State = State.New};

            _context.Tasks.Add(entity);
            _context.SaveChanges();

            response = Created;
        }

        return (response, entity.Id);
    }

    public Response Delete(int taskId)
    {
        var entity = _context.Tasks.Find(taskId); 
        if (entity is null) 
        {
            return NotFound;
        }
        
        Response response;
        switch (entity.State)
        {
            case New:
            _context.Tasks.Remove(entity);
            _context.SaveChanges();

            response = Deleted;
            break;
            case Active:
            entity.State = Removed;
            _context.SaveChanges();
            
            response = Deleted;
            break;
            default:
            response = Conflict;
            break;
        }
        return response;
    }

    public TaskDetailsDTO Read(int taskId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAll()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
    {
        throw new NotImplementedException();
    }

    public Response Update(TaskUpdateDTO task)
    {
        throw new NotImplementedException();
    }
}
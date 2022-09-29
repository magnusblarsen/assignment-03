namespace Assignment3.Core;

public interface ITaskRepository
{
    (Response Response, int TaskId) Create(TaskCreateDTO task);
    IReadOnlyCollection<TaskDTO> ReadAll();
    IReadOnlyCollection<TaskDTO> ReadAllRemoved();
    IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag);
    IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId);
    IReadOnlyCollection<TaskDTO> ReadAllByState(State state);
    TaskDetailsDTO? Read(int taskId);
    Response Update(TaskUpdateDTO task);
    Response Delete(int taskId);
}

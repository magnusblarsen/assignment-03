namespace Assignment3.Entities;
using static Assignment3.Core.Response;
public class UserRepository : IUserRepository
{
    private KanbanContext _context;
    public UserRepository(KanbanContext context) => _context = context;
    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        Response response;

        if(entity is not null)
        {
            response = Conflict;
        } else 
        {
            entity = new User{Name = user.Name, Email = user.Email};

            _context.Users.Add(entity);
            _context.SaveChanges();

            response = Created;
        }

        return (response, entity.Id);
    }

    public Response Delete(int userId, bool force = false)
    {  
        var entity = _context.Users.FirstOrDefault(u => u.Id == userId);
        Response response;

        if(entity is null)
        {
            response = NotFound;
        } 
        else if(entity.Tasks.Any() && !force)
        {
            response = Conflict;
        } 
        else 
        {
            _context.Users.Remove(entity);
            _context.SaveChanges();

            response = Deleted;
        }
        return response;
    }

    public UserDTO Read(int userId)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Id == userId);
        return (entity is null ? null : new UserDTO(entity.Id, entity.Name, entity.Email))!;
    }

    public IReadOnlyCollection<UserDTO> ReadAll() => _context.Users.Select(u => new UserDTO(u.Id, u.Name, u.Email)).ToList().AsReadOnly(); 

    public Response Update(UserUpdateDTO user)
    {
        var entity = _context.Users.Find(user.Id);
        Response res;
        if(entity is null)
        {
            res = NotFound;
        } 
        else 
        {
            entity.Email = user.Email;
            entity.Name = user.Name;
            _context.SaveChanges();

            res = Updated;
        }
        return res;
    }
}

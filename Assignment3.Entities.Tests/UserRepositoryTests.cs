namespace Assignment3.Entities.Tests;
using Microsoft.Data.Sqlite;
public class UserRepositoryTests
{
    private KanbanContext _context;
    private UserRepository _repo;
    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<KanbanContext>()
                        .UseSqlite(connection)
                        .Options;

        var context = new KanbanContext(options);
        context.Database.EnsureCreated();
        //context.Database.EnsureDeleted();

        var tags = new Tag[]
        {
               new Tag{Id = 1, Name = "Discrete Maths", Tasks = new List<WorkItem>()},
               new Tag{Id = 2, Name = "BDSA", Tasks = new List<WorkItem>()}
        };

        User userWithTask = new User
        {
            Id = 3, Name = "Hjul",Email = "hjul@email.com",Tasks = new List<WorkItem>(),
        };
        WorkItem workItemForUserWithTask = new WorkItem
        {
            Id = 1, Title = "Ryd Sne", State = State.New, Tags = new List<Tag>(),
            AssignedTo = userWithTask,
        };
        userWithTask.Tasks.Add(workItemForUserWithTask);

        var users = new User[]
        {
            new User{Id=1, Name = "Tobias", Email = "noget@andet.dk"},
            new User{Id=2, Name = "Magnus", Email = "magnus@noget.dk"},
            userWithTask,
        };

        var workItems = new WorkItem[]
        {
            workItemForUserWithTask,
            new WorkItem{Id = 2, Title="Assignement03", State = State.Active, Tags = new List<Tag>(), Description = "", AssignedTo = users[0]},
        };

        context.Users.AddRange(users);
        context.Tags.AddRange(tags);
        context.Tasks.AddRange(workItems);

        context.SaveChanges();

        _context = context;
        _repo = new UserRepository(_context);
    }
    
    [Fact]
    public void Delete_User_assigned_to_work_item_should_fail()
    {
        // Arrange

        // Act
        var res = _repo.Delete(3);

        // Assert
        Assert.Equal(Response.Conflict, res);
    }
    
    [Fact]
    public void Delete_User_assigned_to_work_item_should_work()
    {
        // Arrange

        // Act
        var res = _repo.Delete(3, true);

        // Assert
        Assert.Equal(Response.Deleted, res);
    }

    [Fact]
    public void Delete_User_in_use_returns_conflict()
    {
        var res = _repo.Delete(3);

        Assert.Equal(Response.Conflict, res);
    }

    [Fact]
    public void Delete_User_in_use_returns_deleted()
    {
        var res = _repo.Delete(3);

        Assert.Equal(Response.Deleted, res);
    }

    [Fact]
    public void Create_User_email_already_exists_returns_conflict()
    {
        // Arrange
        var newUser = new UserCreateDTO("BalladeMager", "noget@andet.dk");

        // Act
        var (response, userId) = _repo.Create(newUser);

        // Assert
        Assert.Equal(Response.Conflict, response);
    }
}

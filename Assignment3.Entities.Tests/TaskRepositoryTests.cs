namespace Assignment3.Entities.Tests;

public class TaskRepositoryTests
{
    private KanbanContext _context;
    private TaskRepository _repo;

    public TaskRepositoryTests()
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
        _repo = new TaskRepository(_context);
    }

    [Fact]
    public void Create_Task_which_already_exists()
    {
        // Arrange
        var taskDTO = new TaskCreateDTO("Assignement03", 1, null, new string[]{"Tobias"});

        // Act
        var (response, id) = _repo.Create(taskDTO);

        // Assert
        Assert.Equal(Response.Conflict, response);
    }

    [Fact]
    public void Delete_non_existing_task()
    {
        var res = _repo.Delete(12000);

        Assert.Equal(Response.NotFound, res);
    }

    [Fact]
    public void Read_existing_Task_returns_non_null()
    {
        // Arrange
        var id = 1;

        // Act
        var result = _repo.Read(id);
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Read_non_existing_Task_returns_null()
    {
        // Arrange
        var id = 52;

        // Act
        var result = _repo.Read(id);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Update_non_existing_task()
    {
        // Arrange
        var updatedTask = new TaskUpdateDTO(12000, "Bingeling", 0, null, new string[]{}, State.Closed);
        
        // Act
        var res = _repo.Update(updatedTask);

        // Assert
        Assert.Equal(Response.NotFound, res);
    }

    [Fact]
    public void Update_tag_returns_updated()
    {
        //Arrange
        var updatedTask = new TaskUpdateDTO(1, "Ryd Sne", 3, "doneso",new string[]{}, State.Closed);

        //Act
        var response = _repo.Update(updatedTask);

        //Assert
        Assert.Equal(Response.Updated, response);
        
    }

    [Theory]
    [InlineData(1,"Assignement03", "Hjul", " ", State.New)]
    [InlineData(2, "Assignement03", "Tobias", " ", State.Active)]
    public void ReadAll_tasks(int id, string name, string assignedToName, string tags, State state)
    {
        var res = _repo.ReadAll();
        
        Assert.Contains(res, t => t == new TaskDTO(id, name, assignedToName, tags.Split(" "), state));
    }

}

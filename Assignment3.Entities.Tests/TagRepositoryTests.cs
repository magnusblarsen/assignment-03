namespace Assignment3.Entities.Tests;

public class TagRepositoryTests
{
    private KanbanContext _context;
    private TagRepository _repo;

    public TagRepositoryTests()
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
        _repo = new TagRepository(_context);
    }

    [Fact]
    public void Create_Tag_which_already_exists()
    {
        // Arrange
        var tagCreateDTO = new TagCreateDTO("Discrete Maths");

        // Act
        var (response, id) = _repo.Create(tagCreateDTO);

        // Assert
        Assert.Equal(Response.Conflict, response);
    }
    

    [Fact]
    public void Read_existing_Tag_returns_non_null()
    {
        // Arrange
        var id = 2;

        // Act
        var result = _repo.Read(id);
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Read_non_existing_Tag_returns_null()
    {
        // Arrange
        var id = 52;

        // Act
        var result = _repo.Read(id);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Delete_tag_in_use_or_assigned_to_task_returns_conflict()
    {
        // arrange
        var wi = new WorkItem{Id = 3, 
                                Title="bingbong",
                                State = State.Active, 
                                Tags = new List<Tag>(),
                                AssignedTo= new User{Name="bob", Email="asd@asd.com"}};
                                
        var tag = new Tag{Id = 4, Name="Later", Tasks = new List<WorkItem>{wi}};
        wi.Tags.Add(tag);
        _context.Tasks.Add(wi);
        _context.Tags.Add(tag);
        _context.SaveChanges();
        // act
        var res = _repo.Delete(4);

        // Assert
        Assert.Equal(Response.Conflict, res);
    }
    

    [Fact]
    public void Delete_tag_assigned_to_workitem_only_using_force()
    {
        // Arrange
        var item = new WorkItem{Id = 4, Title="bongbing", Tags = new List<Tag>(), AssignedTo=new User{Name="bib", Email="dslkjf@lksdjf.dk"}};
        var tag = new Tag{Id= 5, Name="Math", Tasks= new List<WorkItem>{item}};
        _context.Tasks.Add(item);
        _context.Tags.Add(tag);
        _context.SaveChanges();

        // Act
        var response = _repo.Delete(5, true); 

        // Assert
        Assert.Equal(Response.Deleted, response);
    }
    [Fact]
    public void Delete_non_existing_tag()
    {
        var res = _repo.Delete(12000);

        Assert.Equal(Response.NotFound, res);
    }

    public void Update_non_existing_tag()
    {
        // Arrange
        var updatedTag = new TagUpdateDTO(12000, "Bingeling");
        
        // Act
        var res = _repo.Update(updatedTag);

        // Assert
        Assert.Equal(Response.NotFound, res);
    }

    [Fact]
    public void Read_returns_tag_with_id_1()
    {
        var tag = _repo.Read(1);

        Assert.Equal(new TagDTO(1,"Discrete Maths"), tag);
    }

    [Fact]
    public void Read_returns_null()
    {
        var tag = _repo.Read(1200);
        
        Assert.Equal(null, tag);
    }

    [Fact]
    public void Update_tag_returns()
    {
        //Arrange
        var item = new WorkItem{Id = 89, Title="en titel", Tags = new List<Tag>(), AssignedTo=new User{Name="bisjdkfljdsfb", Email="dslkjf@lksdsdfsdfjf.dk"}};
        var tag = new Tag{Id= 89, Name="noget", Tasks= new List<WorkItem>{item}};
        _context.Tasks.Add(item);
        _context.Tags.Add(tag);
        _context.SaveChanges();

        //Act
        var response = _repo.Update(new TagUpdateDTO(89, "et normalt navn"));

        //Assert
        Assert.Equal(Response.Updated, response);
        
    }
    
    [Theory]
    [InlineData(1,"Discrete Maths")]
    [InlineData(2, "BDSA")]
    public void ReadAll_s(int id, string name)
    {
        var res = _repo.ReadAll();
        
        Assert.Contains(res, t => t == new TagDTO(id, name));
    }
}

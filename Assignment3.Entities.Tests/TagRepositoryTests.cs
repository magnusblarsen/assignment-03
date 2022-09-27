namespace Assignment3.Entities.Tests;

public class TagRepositoryTests
{
    private KanbanContext _context;
    private TagRepository _repo;

    public TagRepositoryTests()
    {
        var contextOptions = new DbContextOptionsBuilder<KanbanContext>()
            .UseInMemoryDatabase("KanbanDatabase")
            .Options;

        using var context = new KanbanContext(contextOptions);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var tags = new Tag[]{
               new Tag{Name = "Discrete Maths", Tasks = new List<WorkItem>()},
               new Tag{Name = "BDSA", Tasks = new List<WorkItem>()}
        };
        context.Tags.AddRange(tags);

        var users = new User[]
        {
            new User{Name = "Tobias", Email = "noget@andet.dk"},
            new User{Name = "Magnus", Email = "magnus@noget.dk"}
        };


        var workItems = new WorkItem[]{
            new WorkItem{Title="Assignement03", State = State.Active, Tags = new List<Tag>{tags[0]}, Description = ""}
        };
        context.Tasks.AddRange(workItems);

        
        context.SaveChanges();

        _context = context;
        _repo = new TagRepository(_context);
    }

    [Fact]
    public void Create_Tag_which_already_exists()
    {
        // Arrange

        // Act
        var (response, id) = _repo.Create(new TagCreateDTO("Discrete Maths"));

        // Assert
        Assert.Equal(Response.Conflict, response);
    }
    

    [Fact]
    public void Read_a_Tag()
    {

    }
}

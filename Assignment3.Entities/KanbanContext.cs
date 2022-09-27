namespace Assignment3.Entities;

public class KanbanContext : DbContext
{
    public virtual DbSet<WorkItem> Tasks => Set<WorkItem>();
    public virtual DbSet<User> Users => Set<User>();
    public virtual DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var taskEntity = modelBuilder.Entity<WorkItem>();
        taskEntity.Property(t => t.Title).HasMaxLength(100).IsRequired();
        taskEntity.Property(t => t.State).IsRequired();
        taskEntity.HasMany(task => task.Tags).WithMany(tag => tag.Tasks);
        taskEntity.Property(e => e.State)
                    .HasConversion(
                        v => v.ToString(),
                        v => (State)Enum.Parse(typeof(State), v));

        var userEntity = modelBuilder.Entity<User>();
        userEntity.Property(u => u.Name).HasMaxLength(100).IsRequired();
        userEntity.Property(u => u.Email).HasMaxLength(100).IsRequired();
        userEntity.HasMany(u => u.Tasks).WithOne(t => t.AssignedTo);
        userEntity.HasIndex(u => u.Email).IsUnique();
        
        var tagEntity = modelBuilder.Entity<Tag>();
        tagEntity.Property(t => t.Name).IsRequired();
        tagEntity.HasIndex(t => t.Name).IsUnique();
    }
    
    public KanbanContext(DbContextOptions options) :base(options){}

}

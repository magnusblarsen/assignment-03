namespace Assignment3.Entities;

public class KanbanContext : DbContext
{
    public virtual DbSet<Task> Tasks => Set<Task>();
    public virtual DbSet<User> Users => Set<User>();
    public virtual DbSet<Tag> Tag => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var taskEntity = modelBuilder.Entity<Task>();
        taskEntity.Property(t => t.Title).HasMaxLength(100).IsRequired();
        taskEntity.Property(t => t.State).IsRequired();
        taskEntity.HasMany(task => task.Tags).WithMany(tag => tag.Tasks);

        var userEntity = modelBuilder.Entity<User>();
        userEntity.Property(u => u.Name).HasMaxLength(100).IsRequired();
        userEntity.Property(u => u.Email).HasMaxLength(100).IsRequired();
        userEntity.HasMany(u => u.Tasks).WithOne(t => t.AssignedTo);
        userEntity.HasIndex(u => u.Email).IsUnique();
        
        var tagEntity = modelBuilder.Entity<Tag>();
        tagEntity.Property(t => t.Name).IsRequired();
        tagEntity.HasIndex(t => t.Name).IsUnique();
    }

}

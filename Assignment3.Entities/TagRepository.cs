namespace Assignment3.Entities;
using static Assignment3.Core.Response;
public class TagRepository : ITagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
        => _context = context;

    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        var entity = _context.Tags.FirstOrDefault(t => t.Name == tag.Name);
        Response response;

        if (entity is not null) 
        {
            response = Conflict;
        }
        else
        {
            entity = new Tag{Name = tag.Name};

            _context.Tags.Add(entity);
            _context.SaveChanges();

            response = Created;
        }

        return (response, entity.Id);
    }

    public Response Delete(int tagId, bool force = false)
    {
        var entity = _context.Tags.FirstOrDefault(t => t.Id == tagId);
        Response response;

        if (entity is null) 
        {
            response = NotFound;
        }
        else if (entity.Tasks.Any() && !force)
        {
            response = Conflict;
        }
        else 
        {
            _context.Tags.Remove(entity);
            _context.SaveChanges();

            response = Deleted;
        }

        return response;
    }

    public TagDTO Read(int tagId)
    {
        var entity = _context.Tags.Find(tagId);
        return (entity is null ? null : new TagDTO(entity.Id, entity.Name))!;
    }

    public IReadOnlyCollection<TagDTO> ReadAll() => _context.Tags.Select(t => new TagDTO(t.Id, t.Name)).ToArray();

    public Response Update(TagUpdateDTO tag)
    {
        var entity = _context.Tags.Find(tag.Id);
        Response response;

        if (entity is null) 
        {
            response = NotFound;
        }
        else
        {
            entity.Name = tag.Name;
            _context.SaveChanges();

            response = Updated;
        }

        return response;
    }
}

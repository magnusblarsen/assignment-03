using Assignment3.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var factory = new KanbanContextFactory();
using var context = factory.CreateDbContext(args);

context.Users.Add(new User{Name = "Magnus", Email = "magnus@tblit.dk", Tasks = new List<Assignment3.Entities.Task>()});
context.SaveChanges();
var users = from c in context.Users
                 where c.Name.Contains("Magnus")
                 orderby c.Name
                 select new
                 {
                    c.Name,
                    c.Email,
                    c.Tasks,
                 };

foreach (var user in users)
{
    Console.WriteLine(user);
}
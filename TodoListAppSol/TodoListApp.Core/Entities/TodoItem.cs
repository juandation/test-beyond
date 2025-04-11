using System.Linq;

namespace TodoListApp.Core.Entities;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<Progression> Progressions { get; private set; } = new List<Progression>();

    public bool IsCompleted => Progressions.Sum(p => p.Percent) >= 100;

 
    // public TodoItem(int id, string title, string description, string category)
    // {
    //     Id = id;
    //     Title = title;
    //     Description = description;
    //     Category = category;
    // }
}

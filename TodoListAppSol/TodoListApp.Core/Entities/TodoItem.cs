using System.Collections.Generic;
using System.Linq;

namespace TodoListApp.Core.Entities;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<Progression> Progressions { get; private set; } = new List<Progression>();

    public int Progression => (int)Progressions.Sum(p => p.Percent);

    public bool IsCompleted => Progressions.Sum(p => p.Percent) >= 100;
}

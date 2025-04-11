using TodoListApp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TodoListApp.Core.Services;

public class InMemoryTodoListRepository : ITodoListRepository
{
    private static int _nextId = 1;
    private static readonly List<string> _categories = new List<string> { "Work", "Personal", "Study" };

    public List<string> GetAllCategories()
    {
        return _categories.ToList(); // Return a copy
    }

    public int GetNextId()
    {
        return _nextId++;
    }
}

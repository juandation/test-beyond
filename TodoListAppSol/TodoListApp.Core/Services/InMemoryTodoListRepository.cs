using TodoListApp.Core.Interfaces;

namespace TodoListApp.Core.Services;

public class InMemoryTodoListRepository : ITodoListRepository
{
    private static int _nextId = 1; 
    private readonly List<string> _categories = new List<string> { "Work", "Personal", "Shopping", "Other" }; 

    public int GetNextId()
    {
       
        return _nextId++;
    }

    public List<string> GetAllCategories()
    {
      
        return new List<string>(_categories);
    }
}

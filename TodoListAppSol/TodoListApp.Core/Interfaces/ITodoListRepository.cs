namespace TodoListApp.Core.Interfaces;

public interface ITodoListRepository
{
    int GetNextId();
    List<string> GetAllCategories();
}

using System.Text;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Interfaces;

namespace TodoListApp.Core.Services;

public class TodoListService : ITodoList
{
    private readonly List<TodoItem> _todoItems = new List<TodoItem>();
    private readonly ITodoListRepository _repository;

    public TodoListService(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public void AddItem(int id, string title, string description, string category)
    {
        var validCategories = _repository.GetAllCategories();
        if (!validCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Error: Category '{category}' is not valid.");
            return;
        }

        if (_todoItems.Any(item => item.Id == id))
        {
            Console.WriteLine($"Error: TodoItem with ID {id} already exists.");
            return;
        }

        var newItem = new TodoItem
        {
            Id = id,
            Title = title,
            Description = description,
            Category = category
        };
        _todoItems.Add(newItem);
    }

    public void UpdateItem(int id, string description)
    {
        var item = FindItemById(id);
        if (item == null) return;

        if (item.Progressions.Sum(p => p.Percent) > 50)
        {
            Console.WriteLine($"Error: Cannot update TodoItem {id} because completion is over 50%.");
            return;
        }

        item.Description = description;
    }

    public void RemoveItem(int id)
    {
        var item = FindItemById(id);
        if (item == null) return;

        if (item.Progressions.Sum(p => p.Percent) > 50)
        {
            Console.WriteLine($"Error: Cannot remove TodoItem {id} because completion is over 50%.");
            return;
        }

        _todoItems.Remove(item);
    }

    public void RegisterProgression(int id, DateTime dateTime, decimal percent)
    {
        var item = FindItemById(id);
        if (item == null) return;

        if (percent <= 0 || percent >= 100)
        {
            Console.WriteLine("Error: Progression percentage must be between 0 and 100.");
            return;
        }

        var currentTotalPercent = item.Progressions.Sum(p => p.Percent);
        if (currentTotalPercent + percent > 100)
        {
            Console.WriteLine("Error: Adding this progression would exceed 100% completion.");
            return;
        }

        var lastProgressionDate = item.Progressions.Any() ? item.Progressions.Max(p => p.Date) : DateTime.MinValue;
        if (dateTime <= lastProgressionDate)
        {
            Console.WriteLine("Error: Progression date must be later than the last progression date.");
            return;
        }

        item.Progressions.Add(new Progression { Date = dateTime, Percent = percent });
    }

    public void PrintItems()
    {
        var sortedItems = _todoItems.OrderBy(item => item.Id);

        foreach (var item in sortedItems)
        {
            Console.WriteLine($"{item.Id}) {item.Title} - {item.Description} ({item.Category}) Completed:{item.IsCompleted}");

            decimal accumulatedPercent = 0;
            foreach (var progression in item.Progressions.OrderBy(p => p.Date))
            {
                accumulatedPercent += progression.Percent;
                var progressBar = GenerateProgressBar(accumulatedPercent);
                Console.WriteLine($"   {progression.Date} - {accumulatedPercent}% |{progressBar}|");
            }
        }
    }

    private TodoItem? FindItemById(int id)
    {
        var item = _todoItems.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            Console.WriteLine($"Error: TodoItem with ID {id} not found.");
        }
        return item;
    }

    private string GenerateProgressBar(decimal percentage)
    {
        const int totalWidth = 50; 
        int filledWidth = (int)Math.Round((percentage / 100m) * totalWidth);
        int emptyWidth = totalWidth - filledWidth;

        return new string('O', filledWidth) + new string(' ', emptyWidth);
    }
}

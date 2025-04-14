using TodoListApp.Core.Entities;
using TodoListApp.Core.Interfaces;
using TodoListApp.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoListApp.Core.Services;

public class TodoListService : ITodoList
{
    private readonly List<TodoItem> _items = new();
    private readonly ITodoListRepository _repository;

    public TodoListService(ITodoListRepository repository)
    {
        _repository = repository;
        InitializeDummyData();
    }

    private void InitializeDummyData()
    {
        _items.Clear();

        var validCategories = _repository.GetAllCategories();

        // Item 1
        var item1 = new TodoItem { Id = 1, Title = "Complete project proposal", Description = "Finalize and submit the proposal for the new project.", Category = "Work" };
        _items.Add(item1);

        // Item 2
        var item2 = new TodoItem { Id = 2, Title = "Buy groceries", Description = "Milk, Bread, Eggs, Cheese", Category = "Shopping" };
        item2.Progressions.Add(new Progression { Date = DateTime.UtcNow.AddHours(-2), Percent = 25 });
        _items.Add(item2);

        // Item 3
        var item3 = new TodoItem { Id = 3, Title = "Read chapter 5", Category = "Study" };
        // Add progressions summing to 100 to make it completed
        item3.Progressions.Add(new Progression { Date = DateTime.UtcNow.AddDays(-1), Percent = 100 });
        _items.Add(item3); 

        // Item 4
        var item4 = new TodoItem { Id = 4, Title = "Call Mom", Category = "Personal" };
        item4.Progressions.Add(new Progression { Date = DateTime.UtcNow.AddHours(-4), Percent = 50 });
        _items.Add(item4);

        // Item 5
        var item5 = new TodoItem { Id = 5, Title = "Schedule dentist appointment", Category = "Personal" };
        _items.Add(item5);

     
    }

    public ServiceResult<TodoItem> AddItem(int id, string title, string description, string category)
    {
        if (_items.Any(item => item.Id == id))
        {
            return ServiceResult<TodoItem>.Failure($"TodoItem with Id {id} already exists.");
        }

        var validCategories = _repository.GetAllCategories();
        if (!validCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            return ServiceResult<TodoItem>.Failure($"Category '{category}' is not valid. Valid categories are: {string.Join(", ", validCategories)}.");
        }

        var newItem = new TodoItem
        {
            Id = id,
            Title = title,
            Description = description,
            Category = category
        };

        _items.Add(newItem);
        return ServiceResult<TodoItem>.Success(newItem);
    }

    public ServiceResult UpdateItem(int id, string title, string description, string category)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return ServiceResult.Failure($"TodoItem with Id {id} not found.");
        }

        if (string.IsNullOrWhiteSpace(title)) return ServiceResult.Failure("Title cannot be empty.");
        if (string.IsNullOrWhiteSpace(description)) return ServiceResult.Failure("Description cannot be empty.");

        var validCategories = _repository.GetAllCategories();
        if (!validCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            return ServiceResult.Failure($"Invalid category: {category}. Valid categories are: {string.Join(", ", validCategories)}");
        }

        if (item.Progressions.Sum(p => p.Percent) > 50)
        {
            return ServiceResult.Failure($"Cannot update TodoItem {id} because its progress is over 50%.");
        }

        item.Title = title;
        item.Description = description;
        item.Category = category;

        return ServiceResult.Success();
    }

    public ServiceResult RemoveItem(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return ServiceResult.Failure($"TodoItem with Id {id} not found.");
        }

        if (item.IsCompleted)
        {
            return ServiceResult.Failure($"Cannot remove TodoItem {id} because it is completed.");
        }

        _items.Remove(item);
        return ServiceResult.Success();
    }

    public ServiceResult RegisterProgression(int id, DateTime dateTime, decimal percent)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return ServiceResult.Failure($"TodoItem with Id {id} not found.");
        }

        if (item.IsCompleted)
        {
             return ServiceResult.Failure($"Cannot register progression for TodoItem {id} because it is already completed.");
        }

        if (percent < 0 || percent > 100)
        {
            return ServiceResult.Failure("Percentage must be between 0 and 100.");
        }

        if (item.Progressions.Sum(p => p.Percent) + percent > 100)
        {
            return ServiceResult.Failure("Total percentage cannot exceed 100.");
        }

        if (item.Progressions.Any() && dateTime < item.Progressions.Max(p => p.Date))
        {
            return ServiceResult.Failure("Progression date cannot be earlier than the last registered progression date.");
        }

        item.Progressions.Add(new Progression { Date = dateTime, Percent = percent });
        return ServiceResult.Success();
    }

    public IEnumerable<TodoItem> GetAllItems()
    {
        return _items.OrderBy(i => i.Id).ToList(); 
    }

    public ServiceResult<TodoItem> GetItemById(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return ServiceResult<TodoItem>.Failure($"TodoItem with Id {id} not found.");
        }
        return ServiceResult<TodoItem>.Success(item);
    }

    public void PrintItems()
    {
        if (!_items.Any())
        {
            Console.WriteLine("No TodoItems found.");
            return;
        }

        Console.WriteLine("\n--- Todo List ---");
        foreach (var item in _items.OrderBy(i => i.Id))
        {
            Console.WriteLine($"ID: {item.Id}");
            Console.WriteLine($"  Title: {item.Title}");
            Console.WriteLine($"  Description: {item.Description}");
            Console.WriteLine($"  Category: {item.Category}");
            Console.WriteLine($"  Progress: {item.Progressions.Sum(p => p.Percent):0.##}%{(item.IsCompleted ? " (Completed)" : "")}");
            if (item.Progressions.Any())
            {
                Console.WriteLine("  Progression History:");
                foreach (var prog in item.Progressions.OrderBy(p => p.Date))
                {
                    Console.WriteLine($"    - {prog.Date:yyyy-MM-dd HH:mm}: {prog.Percent}%");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine("--- End of List ---\n");
    }
}
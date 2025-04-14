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
            // CreatedAt = DateTime.Now // Removed - Property doesn't exist
        };

        _items.Add(newItem);
        return ServiceResult<TodoItem>.Success(newItem);
    }

    public ServiceResult UpdateItem(int id, string description)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return ServiceResult.Failure($"TodoItem with Id {id} not found.");
        }

        // Use Sum() instead of CalculateTotalProgress()
        if (item.Progressions.Sum(p => p.Percent) > 50)
        {
            return ServiceResult.Failure($"Cannot update TodoItem {id} because its progress is over 50%.");
        }

        item.Description = description;
        return ServiceResult.Success();
    }

    public ServiceResult RemoveItem(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return ServiceResult.Failure($"TodoItem with Id {id} not found.");
        }

        // IsCompleted is calculated, no change needed here
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

        // IsCompleted is calculated, no change needed here
        if (item.IsCompleted)
        {
             return ServiceResult.Failure($"Cannot register progression for TodoItem {id} because it is already completed.");
        }

        if (percent < 0 || percent > 100)
        {
            return ServiceResult.Failure("Percentage must be between 0 and 100.");
        }

        // Use Sum() instead of CalculateTotalProgress()
        if (item.Progressions.Sum(p => p.Percent) + percent > 100)
        {
            return ServiceResult.Failure("Total percentage cannot exceed 100.");
        }

        // Use Date instead of DateTime
        if (item.Progressions.Any() && dateTime < item.Progressions.Max(p => p.Date))
        {
            return ServiceResult.Failure("Progression date cannot be earlier than the last registered progression date.");
        }

        // Use Date instead of DateTime
        item.Progressions.Add(new Progression { Date = dateTime, Percent = percent });
        // item.IsCompleted = item.Progressions.Sum(p => p.Percent) >= 100; // Removed - IsCompleted is calculated

        return ServiceResult.Success();
    }

    public IEnumerable<TodoItem> GetAllItems()
    {
        return _items.OrderBy(i => i.Id).ToList(); // Return a copy sorted by ID
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
            // No CreatedAt property to print
            // Use Sum() instead of CalculateTotalProgress()
            Console.WriteLine($"  Progress: {item.Progressions.Sum(p => p.Percent):0.##}%{(item.IsCompleted ? " (Completed)" : "")}");
            if (item.Progressions.Any())
            {
                Console.WriteLine("  Progression History:");
                 // Use Date instead of DateTime
                foreach (var prog in item.Progressions.OrderBy(p => p.Date))
                {
                    // Use Date instead of DateTime
                    Console.WriteLine($"    - {prog.Date:yyyy-MM-dd HH:mm}: {prog.Percent}%");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine("--- End of List ---\n");
    }
}
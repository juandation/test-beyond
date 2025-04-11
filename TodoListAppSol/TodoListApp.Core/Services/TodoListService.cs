using TodoListApp.Core.Entities;
using TodoListApp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoListApp.Core.Services;

public class TodoListService : ITodoList
{
    private readonly List<TodoItem> _items = new List<TodoItem>();
    private readonly ITodoListRepository _repository;

    public TodoListService(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public void AddItem(int id, string title, string description, string category)
    {
        if (!_repository.GetAllCategories().Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Error: Category '{category}' is not valid.");
            return;
        }

        if (_items.Any(item => item.Id == id))
        {
            Console.WriteLine($"Error: TodoItem with Id {id} already exists.");
            return;
        }

        var newItem = new TodoItem
        {
            Id = id,
            Title = title,
            Description = description,
            Category = category
        };

        _items.Add(newItem);
        Console.WriteLine($"TodoItem {id} added successfully.");
    }

    public void UpdateItem(int id, string description)
    {
        var itemToUpdate = _items.FirstOrDefault(item => item.Id == id);

        if (itemToUpdate == null)
        {
            Console.WriteLine($"Error: TodoItem with Id {id} not found.");
            return;
        }

        if (itemToUpdate.Progressions.Sum(p => p.Percent) > 50)
        {
            Console.WriteLine($"Error: Cannot update TodoItem {id} because its progress is over 50%.");
            return;
        }

        itemToUpdate.Description = description;
        Console.WriteLine($"TodoItem {id} updated successfully.");
    }

    public void RemoveItem(int id)
    {
        var itemToRemove = _items.FirstOrDefault(item => item.Id == id);

        if (itemToRemove == null)
        {
            Console.WriteLine($"Error: TodoItem with Id {id} not found.");
            return;
        }

        if (itemToRemove.IsCompleted)
        {
            Console.WriteLine($"Error: Cannot remove TodoItem {id} because it is completed.");
            return;
        }

        _items.Remove(itemToRemove);
        Console.WriteLine($"TodoItem {id} removed successfully.");
    }

    public void RegisterProgression(int id, DateTime dateTime, decimal percent)
    {
        var itemToUpdate = _items.FirstOrDefault(item => item.Id == id);

        if (itemToUpdate == null)
        {
            Console.WriteLine($"Error: TodoItem with Id {id} not found.");
            return;
        }

        if (itemToUpdate.IsCompleted)
        {
            Console.WriteLine($"Error: Cannot register progression for TodoItem {id} because it is already completed.");
            return;
        }

        if (percent < 0 || percent > 100)
        {
            Console.WriteLine($"Error: Percentage must be between 0 and 100.");
            return;
        }

        var currentTotalPercent = itemToUpdate.Progressions.Sum(p => p.Percent);
        if (currentTotalPercent + percent > 100)
        {
            Console.WriteLine($"Error: Total percentage cannot exceed 100. Current: {currentTotalPercent}%, Attempted Add: {percent}%.");
            return;
        }

        var lastProgressionDate = itemToUpdate.Progressions.Any() ? itemToUpdate.Progressions.Max(p => p.Date) : DateTime.MinValue;
        if (dateTime < lastProgressionDate)
        {
            Console.WriteLine($"Error: Progression date cannot be earlier than the last registered progression date ({lastProgressionDate:yyyy-MM-dd HH:mm}).");
            return;
        }

        var newProgression = new Progression { Date = dateTime, Percent = percent };
        itemToUpdate.Progressions.Add(newProgression);

        Console.WriteLine($"Progression registered for TodoItem {id}.");
    }

    public void PrintItems()
    {
        if (!_items.Any())
        {
            Console.WriteLine("No TodoItems to display.");
            return;
        }

        Console.WriteLine("\n--- Todo List ---");
        foreach (var item in _items.OrderBy(i => i.Id))
        {
            Console.WriteLine($"Id: {item.Id}");
            Console.WriteLine($"Title: {item.Title}");
            Console.WriteLine($"Description: {item.Description}");
            Console.WriteLine($"Category: {item.Category}");

            decimal totalPercent = item.Progressions.Sum(p => p.Percent);
            string progressBar = GenerateProgressBar(totalPercent);
            Console.WriteLine($"Progress: [{progressBar}] {totalPercent}%");

            Console.WriteLine($"Status: {(item.IsCompleted ? "Completed" : "Pending")}");
            Console.WriteLine("--------------------");
        }
    }

    private string GenerateProgressBar(decimal percentage)
    {
        const int barLength = 20;
        int filledLength = (int)Math.Round((percentage / 100m) * barLength);
        int emptyLength = barLength - filledLength;

        filledLength = Math.Min(filledLength, barLength);
        emptyLength = Math.Max(0, barLength - filledLength);

        return new string('#', filledLength) + new string('-', emptyLength);
    }
}

using TodoListApp.Core.Entities;
using TodoListApp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TodoListApp.Core.Services;

public class InMemoryTodoListRepository : ITodoListRepository
{
    private readonly List<TodoItem> _items = new();
    private readonly List<string> _categories = new() { "Work", "Personal", "Shopping", "Study", "Errands", "Other" }; // Fixed categories
    private int _nextId = 1;

    public InMemoryTodoListRepository() { }

    public IEnumerable<TodoItem> GetAll() => _items;

    public TodoItem? GetById(int id) => _items.FirstOrDefault(item => item.Id == id);

    public void Add(TodoItem item)
    {
        item.Id = GetNextId(); // Ensure new items get a unique ID from the repository
        _items.Add(item);
    }

    public void Update(TodoItem item)
    {
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index != -1)
        {
            _items[index] = item;
        }
    }

    public void Remove(int id)
    {
        _items.RemoveAll(item => item.Id == id);
    }

    public int GetNextId()
    {
        // Simple incrementing ID. In a real scenario, this might need locking or a better strategy.
        return _nextId++;
    }

    public bool IsValidCategory(string category)
    {
        return _categories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }

    public List<string> GetAllCategories() // Change return type to List<string>
    {
        return _categories; // Return the fixed list (which is already List<string>)
    }
}

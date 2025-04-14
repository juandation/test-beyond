using TodoListApp.Core.Entities;
using TodoListApp.Core.Results;
using System;
using System.Collections.Generic;

namespace TodoListApp.Core.Interfaces;

public interface ITodoList
{
    ServiceResult<TodoItem> AddItem(int id, string title, string description, string category);
    ServiceResult UpdateItem(int id, string title, string description, string category);
    ServiceResult RemoveItem(int id);
    ServiceResult RegisterProgression(int id, DateTime dateTime, decimal percent);
    IEnumerable<TodoItem> GetAllItems();
    ServiceResult<TodoItem> GetItemById(int id);
    void PrintItems(); // Keep for console app compatibility
}

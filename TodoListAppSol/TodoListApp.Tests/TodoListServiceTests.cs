using TodoListApp.Core.Interfaces;
using TodoListApp.Core.Services;
using Xunit;
using System.IO;
using System;

namespace TodoListApp.Tests;

public class TodoListServiceTests
{
    private readonly ITodoListRepository _repository;
    private readonly ITodoList _service;

    public TodoListServiceTests()
    {
        _repository = new InMemoryTodoListRepository();
        _service = new TodoListService(_repository);
    }

    [Fact]
    public void Can_AddItem_With_ValidData()
    {
        int initialId = _repository.GetNextId();
        string title = "Test Title";
        string description = "Test Description";
        string category = "Personal";

        _service.AddItem(initialId, title, description, category);

        int nextId = _repository.GetNextId();
        Assert.Equal(initialId + 1, nextId);
    }

    [Fact]
    public void Cannot_AddItem_With_InvalidCategory()
    {
        int id = _repository.GetNextId();
        string invalidCategory = "InvalidCategory";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.AddItem(id, "Test Title", "Test Desc", invalidCategory);

        var output = stringWriter.ToString();
        Assert.Contains($"Error: Category '{invalidCategory}' is not valid.", output);
        Assert.Equal(id + 1, _repository.GetNextId());

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_AddItem_With_DuplicateId()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "First Item", "Desc1", "Personal");

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.AddItem(id, "Second Item", "Desc2", "Work");

        var output = stringWriter.ToString();
        Assert.Contains($"Error: TodoItem with Id {id} already exists.", output);
        Assert.Equal(id + 1, _repository.GetNextId());

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Can_UpdateItem_When_ProgressIsLow()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Update Test", "Initial Desc", "Work");
        _service.RegisterProgression(id, DateTime.Now, 40);
        string newDescription = "Updated Description";

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.UpdateItem(id, newDescription);

        var output = stringWriter.ToString();
        Assert.Contains($"TodoItem {id} updated successfully.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_UpdateItem_When_ProgressIsHigh()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Update Fail Test", "Initial Desc", "Work");
        _service.RegisterProgression(id, DateTime.Now, 60);
        string newDescription = "Should Not Update";

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.UpdateItem(id, newDescription);

        var output = stringWriter.ToString();
        Assert.Contains($"Error: Cannot update TodoItem {id} because its progress is over 50%.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_UpdateItem_When_ItemNotFound()
    {
        int nonExistentId = 999;
        string newDescription = "Doesn't Matter";

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.UpdateItem(nonExistentId, newDescription);

        var output = stringWriter.ToString();
        Assert.Contains($"Error: TodoItem with Id {nonExistentId} not found.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Can_RemoveItem_When_NotCompleted()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Remove Test", "Item to remove", "Personal");
        _service.RegisterProgression(id, DateTime.Now, 30);

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RemoveItem(id);

        var output = stringWriter.ToString();
        Assert.Contains($"TodoItem {id} removed successfully.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_RemoveItem_When_Completed()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Remove Fail Test", "Completed item", "Work");
        _service.RegisterProgression(id, DateTime.Now.AddDays(-1), 100);

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RemoveItem(id);

        var output = stringWriter.ToString();
        Assert.Contains($"Error: Cannot remove TodoItem {id} because it is completed.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_RemoveItem_When_ItemNotFound()
    {
        int nonExistentId = 888;

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RemoveItem(nonExistentId);

        var output = stringWriter.ToString();
        Assert.Contains($"Error: TodoItem with Id {nonExistentId} not found.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Can_RegisterProgression_With_ValidData()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Progress Test", "Desc", "Study");
        decimal percent = 45;
        DateTime date = DateTime.Now;

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RegisterProgression(id, date, percent);

        var output = stringWriter.ToString();
        Assert.Contains($"Progression registered for TodoItem {id}.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_RegisterProgression_When_ItemNotFound()
    {
        int nonExistentId = 777;

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RegisterProgression(nonExistentId, DateTime.Now, 50);

        var output = stringWriter.ToString();
        Assert.Contains($"Error: TodoItem with Id {nonExistentId} not found.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_RegisterProgression_When_ItemIsCompleted()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Progress Fail Test", "Completed item", "Personal");
        _service.RegisterProgression(id, DateTime.Now.AddDays(-1), 100);

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RegisterProgression(id, DateTime.Now, 10);

        var output = stringWriter.ToString();
        Assert.Contains($"Error: Cannot register progression for TodoItem {id} because it is already completed.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(110)]
    public void Cannot_RegisterProgression_With_InvalidPercentage(decimal invalidPercent)
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Progress Percent Test", "Desc", "Work");

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RegisterProgression(id, DateTime.Now, invalidPercent);

        var output = stringWriter.ToString();
        Assert.Contains("Error: Percentage must be between 0 and 100.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_RegisterProgression_When_TotalExceeds100()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Progress Total Test", "Desc", "Study");
        _service.RegisterProgression(id, DateTime.Now.AddDays(-1), 70);

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RegisterProgression(id, DateTime.Now, 40);

        var output = stringWriter.ToString();
        Assert.Contains("Error: Total percentage cannot exceed 100.", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    [Fact]
    public void Cannot_RegisterProgression_With_EarlierDate()
    {
        int id = _repository.GetNextId();
        DateTime firstDate = DateTime.Now.AddDays(-2);
        DateTime earlierDate = DateTime.Now.AddDays(-3);
        _service.AddItem(id, "Progress Date Test", "Desc", "Personal");
        _service.RegisterProgression(id, firstDate, 30);

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _service.RegisterProgression(id, earlierDate, 20);

        var output = stringWriter.ToString();
        Assert.Contains("Error: Progression date cannot be earlier than the last registered progression date", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }
}

using Xunit;
using TodoListApp.Core.Services;
using TodoListApp.Core.Interfaces;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Results;
using System;
using System.IO;
using System.Linq;

namespace TodoListApp.Tests;

public class TodoListServiceTests : IDisposable
{
    private readonly InMemoryTodoListRepository _repository;
    private readonly ITodoList _service;
    private readonly StringWriter _stringWriter;
    private readonly TextWriter _originalOutput;

    public TodoListServiceTests()
    {
        _repository = new InMemoryTodoListRepository();
        _service = new TodoListService(_repository);
        _stringWriter = new StringWriter();
        _originalOutput = Console.Out;
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _stringWriter.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Can_AddItem_With_ValidData()
    {
        int id = _repository.GetNextId();
        string title = "Test Task";
        string desc = "Test Description";
        string category = "Personal";

        var result = _service.AddItem(id, title, desc, category);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(title, result.Value.Title);
        Assert.Equal(desc, result.Value.Description);
        Assert.Equal(category, result.Value.Category);
        Assert.Equal(id + 1, _repository.GetNextId());

        var addedItemResult = _service.GetItemById(id);
        Assert.True(addedItemResult.IsSuccess);
        Assert.NotNull(addedItemResult.Value);
        Assert.Equal(title, addedItemResult.Value.Title);
    }

    [Fact]
    public void Cannot_AddItem_With_InvalidCategory()
    {
        int id = _repository.GetNextId();
        string invalidCategory = "InvalidCategory";

        var result = _service.AddItem(id, "Test Title", "Test Desc", invalidCategory);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Contains("not valid", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);

        var getItemResult = _service.GetItemById(id);
        Assert.False(getItemResult.IsSuccess);
    }

    [Fact]
    public void Cannot_AddItem_With_DuplicateId()
    {
        int id = _repository.GetNextId();
        var firstAddResult = _service.AddItem(id, "First Item", "Desc1", "Personal");
        Assert.True(firstAddResult.IsSuccess);
        Assert.Equal(id + 1, _repository.GetNextId());

        var secondAddResult = _service.AddItem(id, "Second Item", "Desc2", "Work");

        Assert.False(secondAddResult.IsSuccess);
        Assert.Null(secondAddResult.Value);
        Assert.Contains("already exists", secondAddResult.ErrorMessage, StringComparison.OrdinalIgnoreCase);

        var allItems = _service.GetAllItems();
        Assert.Single(allItems);
        Assert.Equal("First Item", allItems.First().Title);
    }

    [Fact]
    public void Can_UpdateItem_With_ValidData_And_ProgressBelow50()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Update Test", "Initial Desc", "Work");
        _service.RegisterProgression(id, DateTime.Now, 30);

        string newDesc = "Updated Description";
        var result = _service.UpdateItem(id, newDesc);

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);

        var updatedItemResult = _service.GetItemById(id);
        Assert.True(updatedItemResult.IsSuccess);
        Assert.NotNull(updatedItemResult.Value);
        Assert.Equal(newDesc, updatedItemResult.Value.Description);
    }

    [Fact]
    public void Cannot_UpdateItem_When_ProgressOver50()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Update Fail Test", "Initial Desc", "Work");
        _service.RegisterProgression(id, DateTime.Now, 60);

        string newDesc = "Updated Description";
        var result = _service.UpdateItem(id, newDesc);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("progress is over 50%", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);

        var itemResult = _service.GetItemById(id);
        Assert.True(itemResult.IsSuccess);
        Assert.NotNull(itemResult.Value);
        Assert.Equal("Initial Desc", itemResult.Value.Description);
    }

    [Fact]
    public void Cannot_UpdateItem_When_NotFound()
    {
        int nonExistentId = 999;
        var result = _service.UpdateItem(nonExistentId, "New Description");

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Can_RemoveItem_When_Exists_And_NotCompleted()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Remove Test", "Desc", "Personal");
        _service.RegisterProgression(id, DateTime.Now, 40);

        var result = _service.RemoveItem(id);

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);

        var getItemResult = _service.GetItemById(id);
        Assert.False(getItemResult.IsSuccess);
        Assert.Contains("not found", getItemResult.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cannot_RemoveItem_When_Completed()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Remove Fail Test", "Desc", "Personal");
        _service.RegisterProgression(id, DateTime.Now, 100);

        var result = _service.RemoveItem(id);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("is completed", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);

        var getItemResult = _service.GetItemById(id);
        Assert.True(getItemResult.IsSuccess);
        Assert.NotNull(getItemResult.Value);
    }

    [Fact]
    public void Cannot_RemoveItem_When_NotFound()
    {
        int nonExistentId = 999;
        var result = _service.RemoveItem(nonExistentId);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Can_RegisterProgression_With_ValidData()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Progress Test", "Desc", "Work");
        DateTime progTime = DateTime.Now;
        decimal percent = 25;

        var result = _service.RegisterProgression(id, progTime, percent);

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);

        var itemResult = _service.GetItemById(id);
        Assert.True(itemResult.IsSuccess);
        Assert.NotNull(itemResult.Value);
        Assert.Single(itemResult.Value.Progressions);
        // Use Date instead of DateTime
        Assert.Equal(progTime, itemResult.Value.Progressions.First().Date);
        Assert.Equal(percent, itemResult.Value.Progressions.First().Percent);
        Assert.False(itemResult.Value.IsCompleted);
    }

    [Fact]
    public void Can_RegisterProgression_To_Reach100Percent()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Complete Test", "Desc", "Work");
        _service.RegisterProgression(id, DateTime.Now.AddMinutes(-10), 60);
        DateTime finalProgTime = DateTime.Now;
        decimal finalPercent = 40;

        var result = _service.RegisterProgression(id, finalProgTime, finalPercent);

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);

        var itemResult = _service.GetItemById(id);
        Assert.True(itemResult.IsSuccess);
        Assert.NotNull(itemResult.Value);
        Assert.Equal(2, itemResult.Value.Progressions.Count);
        Assert.True(itemResult.Value.IsCompleted);
        // Use Sum() instead of CalculateTotalProgress()
        Assert.Equal(100, itemResult.Value.Progressions.Sum(p => p.Percent));
    }

    [Fact]
    public void Cannot_RegisterProgression_When_NotFound()
    {
        int nonExistentId = 999;
        var result = _service.RegisterProgression(nonExistentId, DateTime.Now, 50);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cannot_RegisterProgression_When_ItemCompleted()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Already Complete", "Desc", "Personal");
        _service.RegisterProgression(id, DateTime.Now, 100);

        var result = _service.RegisterProgression(id, DateTime.Now.AddDays(1), 10);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("already completed", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(110)]
    public void Cannot_RegisterProgression_With_InvalidPercentage(decimal invalidPercent)
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Invalid Percent", "Desc", "Work");

        var result = _service.RegisterProgression(id, DateTime.Now, invalidPercent);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("between 0 and 100", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cannot_RegisterProgression_When_TotalExceeds100()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Exceed 100", "Desc", "Personal");
        _service.RegisterProgression(id, DateTime.Now, 70);

        var result = _service.RegisterProgression(id, DateTime.Now.AddMinutes(5), 40);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("exceed 100", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cannot_RegisterProgression_With_EarlierDateThanLast()
    {
        int id = _repository.GetNextId();
        _service.AddItem(id, "Date Order Test", "Desc", "Work");
        var laterDate = DateTime.Now;
        _service.RegisterProgression(id, laterDate, 30);

        var earlierDate = laterDate.AddMinutes(-10);
        var result = _service.RegisterProgression(id, earlierDate, 20);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("earlier than the last", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
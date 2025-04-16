using Carter;
using TodoListApp.Core.Interfaces;

namespace TodoListApp.Api.Modules;


public record CreateTodoRequest(string Title, string Description, string Category);

public record UpdateTodoDescriptionRequest(string Description);

public record UpdateTodoRequest(string Title, string Description, string Category);

public record AddProgressRequest(DateTime DateTime, decimal Percent);

public class TodoModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos").WithTags("Todos");

        group.MapGet("/", GetAllTodos);
      
        group.MapGet("/{id:int}", GetTodoById);

        group.MapPost("/", CreateTodo);
    
        group.MapPut("/{id:int}", UpdateTodo);

        group.MapDelete("/{id:int}", DeleteTodo);

        group.MapPost("/{id:int}/progress", AddTodoProgress);

        group.MapGet("/categories", GetAllCategories);
    }

 

    private static IResult GetAllTodos(ITodoList todoListService)
    {
        var items = todoListService.GetAllItems();
        return Results.Ok(items);
    }

    private static IResult GetTodoById(int id, ITodoList todoListService)
    {
 
        var result = todoListService.GetItemById(id);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.ErrorMessage);
    }

    private static IResult CreateTodo(CreateTodoRequest request, ITodoList todoListService, ITodoListRepository repository)
    {
      
        var newId = repository.GetNextId(); 
        
        var result = todoListService.AddItem(newId, request.Title, request.Description, request.Category);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.ErrorMessage);
        }

        var newItem = result.Value; 
        return Results.Created($"/todos/{newItem!.Id}", newItem);
    }

    private static IResult UpdateTodo(int id, UpdateTodoRequest request, ITodoList todoListService)
    {
        var result = todoListService.UpdateItem(id, request.Title, request.Description, request.Category);

        if (!result.IsSuccess)
        {
        
            if (result.ErrorMessage!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return Results.NotFound(result.ErrorMessage);
            }
          
            return Results.BadRequest(result.ErrorMessage); 
        }

        return Results.NoContent();
    }

    private static IResult DeleteTodo(int id, ITodoList todoListService)
    {
        var result = todoListService.RemoveItem(id);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return Results.NotFound(result.ErrorMessage);
            }
            return Results.BadRequest(result.ErrorMessage); 
        }

        return Results.NoContent();
    }

    private static IResult AddTodoProgress(int id, AddProgressRequest request, ITodoList todoListService)
    {
        var result = todoListService.RegisterProgression(id, request.DateTime, request.Percent);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return Results.NotFound(result.ErrorMessage);
            }
            return Results.BadRequest(result.ErrorMessage);
        }

        return Results.NoContent();
    }

    private static IResult GetAllCategories(ITodoListRepository repository)
    {
        var categories = repository.GetAllCategories();
        return Results.Ok(categories);
    }
}

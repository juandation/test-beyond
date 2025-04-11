using TodoListApp.Core.Entities; 
using TodoListApp.Core.Interfaces;
using TodoListApp.Core.Services;

// Basic Dependency Injection Setup (manual)
ITodoListRepository repository = new InMemoryTodoListRepository();
ITodoList todoListService = new TodoListService(repository);

// --- Add Dummy Data to test easily--- 
Console.WriteLine("Adding dummy data...");

int id1 = repository.GetNextId();
todoListService.AddItem(id1, "Learn C#", "Complete online tutorial", "Personal");

int id2 = repository.GetNextId();
todoListService.AddItem(id2, "Prepare Presentation", "Create slides for meeting", "Work");
todoListService.RegisterProgression(id2, DateTime.Now.AddDays(-2), 30);

int id3 = repository.GetNextId();
todoListService.AddItem(id3, "Read Book", "Finish novel chapter", "Personal");
todoListService.RegisterProgression(id3, DateTime.Now.AddDays(-5), 50);
todoListService.RegisterProgression(id3, DateTime.Now.AddDays(-1), 25);

int id4 = repository.GetNextId();
todoListService.AddItem(id4, "Plan Project", "Outline main tasks", "Work");

int id5 = repository.GetNextId();
todoListService.AddItem(id5, "Exercise", "Morning run", "Personal");
todoListService.RegisterProgression(id5, DateTime.Now.AddDays(-3), 60);
todoListService.RegisterProgression(id5, DateTime.Now.AddDays(-1), 40);

Console.WriteLine("Dummy data added.");
// --- End Dummy Data ---

Console.WriteLine("--- TodoList Console App ---");
Console.WriteLine("Enter commands (e.g., 'add', 'update', 'remove', 'progress', 'print', 'exit'):");

string? commandLine;
do
{
    Console.Write("> ");
    commandLine = Console.ReadLine();

    if (commandLine == null || commandLine.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    ParseAndExecuteCommand(commandLine, todoListService, repository);

} while (true);

Console.WriteLine("Exiting application.");

static void ParseAndExecuteCommand(string commandLine, ITodoList service, ITodoListRepository repo)
{
    var parts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 0)
    {
        Console.WriteLine("Error: Please enter a command.");
        return;
    }

    var command = parts[0].ToLowerInvariant();
    var args = parts.Skip(1).ToArray();

    try
    {
        switch (command)
        {
            case "add":
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: add <title> <description> <category>");
                    return;
                }
                int newId = repo.GetNextId();
                string title = args[0];
                string description = args[1];
                string category = args[2];
                service.AddItem(newId, title, description, category);
                break;

            case "update":
                if (args.Length < 2 || !int.TryParse(args[0], out int updateId))
                {
                    Console.WriteLine("Usage: update <id> <new_description>");
                    return;
                }
                service.UpdateItem(updateId, args[1]);
                break;

            case "remove":
                if (args.Length < 1 || !int.TryParse(args[0], out int removeId))
                {
                    Console.WriteLine("Usage: remove <id>");
                    return;
                }
                service.RemoveItem(removeId);
                break;

            case "progress":
                if (args.Length < 2 || !int.TryParse(args[0], out int progressId) || !decimal.TryParse(args[1], out decimal percent))
                {
                    Console.WriteLine("Usage: progress <id> <percent> [yyyy-MM-dd HH:mm]"); // Date is optional
                    return;
                }
                DateTime progressDate = args.Length > 2 && DateTime.TryParse(args[2], out DateTime parsedDate) ? parsedDate : DateTime.Now;
                service.RegisterProgression(progressId, progressDate, percent);
                break;

            case "print":
                service.PrintItems();
                break;

            default:
                Console.WriteLine($"Error: Unknown command '{command}'. Valid commands: add, update, remove, progress, print, exit.");
                break;
        }
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"Error parsing arguments: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        
    }
}

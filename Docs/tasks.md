# Senior Developer Exercise - Todo List Application

## Table of Contents

- [Main Task](#main-task)
- [TodoItem Structure](#todoitem-structure)
- [Progression System](#progression-system)
- [Validation Rules](#validation-rules)
- [Output Format](#output-format)
- [Repository Interface](#repository-interface)
- [Compulsory Tasks](#compulsory-tasks)
- [Extra Tasks](#extra-tasks)
- [Delivery](#delivery)

## Main Task

Create a console application that manages a list of TodoItems. The list must be contained within an aggregate that implements this exact interface:

```csharp
public interface ITodoList
{
    void AddItem(int id, string title, string description, string category);
    void UpdateItem(int id, string description);
    void RemoveItem(int id);
    void RegisterProgression(int id, DateTime dateTime, decimal percent);
    void PrintItems();
}
```

## TodoItem Structure

A TodoItem will have the following properties:

- Id
- Title
- Description
- Category
- List of Progressions
- Calculated field: IsCompleted

## Progression System

Each Progression element will have two fields:

- Date: When the action was performed
- Percent: The completed percentage

## Validation Rules

When adding Progressions to a TodoItem:

- The new Progression's date must be later than any existing progressions
- The percentage must be valid (greater than 0 and less than 100)
- The total percentage after adding all progressions must not exceed 100%
- TodoItems cannot be updated or deleted if they have more than 50% completion

## Output Format

When PrintItems is executed, it will display all TodoItems in the aggregate, ordered by Id. The header format for each TodoItem is:

```
{Id}) {Title} - {Description} ({Category}) Completed:{IsCompleted}
```

For each Progression element, it will show:

- Date
- Accumulated percentage
- Progress bar

Example output:

```
1. Complete Project Report - Finish the final report for the project (Work) Completed:True
   3/18/2025 12:00:00 AM - 30% |OOOOOOOOOOOOOOO |
   3/19/2025 12:00:00 AM - 80% |OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO |
   3/20/2025 12:00:00 AM - 100% |OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO|
```

## Repository Interface

A repository interface is required to manage Id generation and category validation:

```csharp
public interface ITodoListRepository
{
    int GetNextId();
    List<string> GetAllCategories();
}
```

## Compulsory Tasks

1. Create a console application in .NET 8 or higher that meets the requirements described above
2. Add unit tests (using your preferred framework) to validate:
   - Happy path
   - The example provided
   - Error cases that should fail

## Extra Tasks

1. Convert the initial console application into a Web API, where a TodoListServer handles requests for managing TodoLists and Progressions
2. Create an asynchronous and dynamic front-end client (preferably in JavaScript or your preferred framework) that:
   - Sends and receives operations without page reloads
   - Displays progress bars with minimal realistic styling (using CSS)
3. Any other enhancements that add value to the project. We love creativity!

## Delivery

Create the project in a public repository using GitHub.

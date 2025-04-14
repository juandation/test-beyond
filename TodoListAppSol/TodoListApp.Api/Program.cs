using Carter;
using TodoListApp.Core.Interfaces;
using TodoListApp.Core.Services;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Carter
builder.Services.AddCarter();

// Register Core Services (using Singleton for in-memory repository and service)
builder.Services.AddSingleton<ITodoListRepository, InMemoryTodoListRepository>();
builder.Services.AddSingleton<ITodoList, TodoListService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map Carter Endpoints
app.MapCarter();

app.Run();

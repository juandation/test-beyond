import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoItem, AddTodoRequest } from '../models/todo-item.model';
import { TodoService } from './services/todo.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  private todoService = inject(TodoService);

  todos: TodoItem[] = [];
  filteredTodos: TodoItem[] = [];
  availableCategories: string[] = [];
  searchQuery: string = '';
  selectedCategory: string = '';
  newTodoTitle: string = '';
  newTodoDescription: string = '';
  newTodoCategory: string = '';
  isAddTodoModalOpen = false;
  isLoading = true;
  loadingError: string | null = null;
  addTodoError: string | null = null;

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.isLoading = true;
    this.loadingError = null;
    this.todoService.getCategories().subscribe({
      next: (categories: string[]) => {
        this.availableCategories = categories;
        this.newTodoCategory = categories.length > 0 ? categories[0] : '';
        this.loadTodos();
      },
      error: (err: HttpErrorResponse | Error) => {
        console.error('Failed to load categories:', err);
        this.loadingError = `Failed to load categories: ${err.message}`;
        this.isLoading = false;
      },
    });
  }

  loadTodos(): void {
    this.todoService.getTodos().subscribe({
      next: (data: TodoItem[]) => {
        this.todos = data;
        this.filterTodos();
        this.loadingError = null;
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse | Error) => {
        console.error('Failed to load todos:', err);
        this.loadingError = `Failed to load tasks: ${err.message}`;
        this.isLoading = false;
      },
    });
  }

  filterTodos(): void {
    let tempTodos = this.todos;

    if (this.selectedCategory) {
      tempTodos = tempTodos.filter(
        (todo) => todo.category === this.selectedCategory
      );
    }

    if (this.searchQuery.trim()) {
      const lowerQuery = this.searchQuery.toLowerCase().trim();
      tempTodos = tempTodos.filter(
        (todo) =>
          todo.title.toLowerCase().includes(lowerQuery) ||
          (todo.description &&
            todo.description.toLowerCase().includes(lowerQuery))
      );
    }

    this.filteredTodos = tempTodos;
  }

  openAddTodoModal(): void {
    this.addTodoError = null;
    this.newTodoCategory =
      this.availableCategories.length > 0 ? this.availableCategories[0] : '';
    this.isAddTodoModalOpen = true;
  }

  closeAddTodoModal(): void {
    this.isAddTodoModalOpen = false;
    this.newTodoTitle = '';
    this.newTodoDescription = '';
    this.newTodoCategory =
      this.availableCategories.length > 0 ? this.availableCategories[0] : '';
  }

  addTodo(): void {
    if (!this.newTodoTitle || !this.newTodoCategory) {
      this.addTodoError = 'Title and Category are required.';
      return;
    }
    this.addTodoError = null;

    const todoData: AddTodoRequest = {
      title: this.newTodoTitle,
      description: this.newTodoDescription,
      category: this.newTodoCategory,
    };

    this.todoService.addTodo(todoData).subscribe({
      next: (newTodo: TodoItem) => {
        this.loadTodos();

        this.closeAddTodoModal();
      },
      error: (err: HttpErrorResponse | Error) => {
        console.error('Error adding todo:', err);
        this.addTodoError = `Failed to add task: ${err.message}`;
      },
    });
  }

  deleteTodo(id: number): void {
    this.todoService.deleteTodo(id).subscribe({
      next: () => {
        this.loadTodos();
      },
      error: (err: HttpErrorResponse | Error) => {
        console.error('Error deleting todo:', err);
        this.loadingError = `Failed to delete task: ${err.message}`;
      },
    });
  }

  addProgression(todoId: number, percent: number): void {
    if (percent == null || percent < 0 || percent > 100) {
      console.error('Invalid percentage value');
      this.loadingError = 'Progress percentage must be between 0 and 100.';
      return;
    }
    this.loadingError = null;
    // Corrected method name based on TodoService definition
    this.todoService.addProgress(todoId, percent).subscribe({
      next: () => {
        this.loadTodos(); // Reload todos after adding progress
      },
      error: (err: HttpErrorResponse | Error) => {
        console.error('Error adding progression:', err);
        this.loadingError = `Failed to add progression: ${err.message}`;
      },
    });
  }
}

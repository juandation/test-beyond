import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TodoService, TodoItem } from './services/todo.service';
import { HttpErrorResponse } from '@angular/common/http'; 

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'TodoListAppClient';
  todos: TodoItem[] = [];
  private todoService = inject(TodoService);

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.todoService.getTodos().subscribe({
      next: (data: TodoItem[]) => { 
        this.todos = data;
        console.log('Todos loaded:', this.todos);
      },
      error: (err: HttpErrorResponse) => console.error('Error loading todos:', err) 
    });
  }

  // Add methods for add, update, delete, progress later
}

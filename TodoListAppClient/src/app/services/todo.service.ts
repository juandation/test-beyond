import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Define an interface for the Todo item structure
export interface TodoItem {
  id: number;
  title: string;
  description: string;
  category: string;
  isCompleted: boolean;
  progression: number;
  creationDate: string; // Assuming ISO string format
  lastUpdateDate: string; // Assuming ISO string format
}

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = 'http://localhost:5056/todos'; // Your API base URL

  constructor(private http: HttpClient) { }

  // Fetch all Todo items
  getTodos(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.apiUrl);
  }

  // Add other methods (addTodo, updateTodo, deleteTodo, addProgress) later
}

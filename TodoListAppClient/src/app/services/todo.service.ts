import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { TodoItem, AddTodoRequest } from '../../models/todo-item.model';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5056/todos';

  public getTodos(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.apiUrl).pipe(
      tap((todos) => {
        console.log('Fetched todos from API:', this.apiUrl);
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error loading todos from API:', error);
        return throwError(
          () => new Error(`Could not load todos from API: ${error.message}`)
        );
      })
    );
  }

  public addTodo(todoData: AddTodoRequest): Observable<TodoItem> {
    console.log('Adding todo via API:', this.apiUrl);
    // Use the actual API endpoint
    return this.http.post<TodoItem>(this.apiUrl, todoData).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error adding todo via API:', error);
        return throwError(
          () => new Error(`Could not add todo via API: ${error.message}`)
        );
      })
    );
  }

  public deleteTodo(id: number): Observable<void> {
    const url = `${this.apiUrl}/${id}`;
    console.log('Deleting todo via API:', url);
    // Use the actual API endpoint
    return this.http.delete<void>(url).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error deleting todo via API:', error);
        return throwError(
          () => new Error(`Could not delete todo via API: ${error.message}`)
        );
      })
    );
  }

  public addProgress(id: number, percent: number): Observable<void> {
    const url = `${this.apiUrl}/${id}/progress`;
    console.log('Adding progress via API:', url);
    // Use the actual API endpoint
    const now = new Date().toISOString();
    const body = { dateTime: now, percent: percent };
    return this.http.post<void>(url, body).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error adding progress via API:', error);
        return throwError(
          () => new Error(`Could not add progress via API: ${error.message}`)
        );
      })
    );
  }

  public getCategories(): Observable<string[]> {
    const categoriesUrl = `${this.apiUrl}/categories`;
    console.log('Fetching categories from API:', categoriesUrl);
    return this.http.get<string[]>(categoriesUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error fetching categories from API:', error);
        return throwError(
          () =>
            new Error(`Could not load categories. API error: ${error.message}`)
        );
      })
    );
  }
}

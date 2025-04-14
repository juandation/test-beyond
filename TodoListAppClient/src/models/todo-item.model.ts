export interface Progression {
  date: string;
  percent: number;
}

export interface TodoItem {
  id: number;
  title: string;
  description: string;
  category: string;
  progressions: Progression[];
  progression: number;
  isCompleted: boolean;
}

// Definition for the data sent to create a new Todo item
export interface AddTodoRequest {
  title: string;
  description: string; // Now mandatory
  category: string;    // Now mandatory
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.dev';
import { Task } from '../models/task';


@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = environment.apiBaseUrl + '/TaskManagment/'; 

  constructor(private http: HttpClient) { }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/GetTasks`);
  }

  createTask(task: Task): Observable<Task> {
    return this.http.post<Task>(`${this.apiUrl}/CreateTask`,task);
  }

  updateTask(task: Task): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/UpdateTask/${task.id}`, task); 
  }

  deleteTask(taskId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Delete/${taskId}`);
  }
}

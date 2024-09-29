import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.dev';
import { CreateTaskDto, TASK_STATUS, UserTaskDto } from '../models/task';


@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = environment.apiBaseUrl + '/TaskManagment'; 
  headers = new HttpHeaders({
    "Content-Type": "application/json",  
  });

  constructor(private http: HttpClient) { }

  getTasks(): Observable<UserTaskDto[]> {
    return this.http.get<UserTaskDto[]>(`${this.apiUrl}/GetTasks`);
  }

  displayStatus(status: TASK_STATUS){
    if(status == TASK_STATUS.COMPLETED) return "Completed";
    if(status == TASK_STATUS.INPROGRESS) return "In Progress";
    if(status == TASK_STATUS.PENDING) return "Pending";
    return "Unknown";
  }

  createTask(task: UserTaskDto): Observable<CreateTaskDto> {
    var {title,description,dueDate,status} = task;    
    var newTask: CreateTaskDto = {title,description, dueDate, status:Number(status)};
    return this.http.post<CreateTaskDto>(`${this.apiUrl}/CreateTask`, JSON.stringify(newTask),{ headers: this.headers});
  }

  updateTask(task: UserTaskDto): Observable<UserTaskDto> {
    var {id,title,description,dueDate,status} = task;    
    var updatedTask = {id,title,description, dueDate, status:Number(status)};
    return this.http.put<UserTaskDto>(`${this.apiUrl}/UpdateTask/${updatedTask.id}`, updatedTask); 
  }

  deleteTask(taskId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/DeleteTask/${taskId}`);
  }

  calculateDueDate(): Date {
    const currentDate = new Date();
    const dueDate = new Date(currentDate.setDate(currentDate.getDate()));
    return dueDate;
  }

}

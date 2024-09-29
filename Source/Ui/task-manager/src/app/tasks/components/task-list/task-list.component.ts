import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { AuthService } from '../../../auth/services/auth.service';
import { TASK_STATUS, UserTaskDto } from '../../models/task';
import { map } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css']
})
export class TaskListComponent implements OnInit {
  tasks: UserTaskDto[] = [];
  taskData: UserTaskDto; // Track the task being edited
  isEditing: boolean = false;
  defaultTaskData = { id: '', title: '', description: '',status: TASK_STATUS.PENDING, dueDate: this.taskService.calculateDueDate() };

  constructor(private taskService: TaskService, private authService: AuthService, private router: Router) {
    this.taskData = this.defaultTaskData;
   }

  
  ngOnInit(): void {
    this.loadTasks();
  }

  displayStatus(status: TASK_STATUS){
    if(status == TASK_STATUS.COMPLETED) return "Completed";
    if(status == TASK_STATUS.INPROGRESS) return "In Progress";
    if(status == TASK_STATUS.PENDING) return "Pending";
    return "Unknown";
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe(tasks => this.tasks = tasks);
  }

  addTask(newTask: UserTaskDto): void {
    this.taskService.createTask(newTask).subscribe(() => {
      this.loadTasks(); // Refresh the task list
    });
  }

  editTask(task: UserTaskDto): void {
    this.taskData = { ...task }; // Create a copy for editing
    this.isEditing = true;
  }

  editCanceled(): void {
    this.isEditing = false;
    this.taskData = this.defaultTaskData;
  }

  updateTask(task: UserTaskDto): void {
    this.taskService.updateTask(task).subscribe(() => {
      this.taskData = this.defaultTaskData;
      this.isEditing = false;
      this.taskData = this.defaultTaskData;
      this.loadTasks(); // Refresh the task list
    });
  }

  deleteTask(taskId: string): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(taskId).subscribe(() => {
        this.loadTasks(); // Refresh the task list
      });
    }
  }

  onLogOut(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']); // Navigate to login on success
      },
      error: (err) => {   
        alert(err);             
      }
    });
  }
}

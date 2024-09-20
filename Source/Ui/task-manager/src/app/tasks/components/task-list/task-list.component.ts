import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task'; // Assuming you have a Task model
import { AuthService } from '../../../auth/services/auth.service';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css']
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  editingTask: Task | null = null; // Track the task being edited

  constructor(private taskService: TaskService, private authService: AuthService) { }

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe(tasks => this.tasks = tasks);
  }

  addTask(newTask: Task): void {
    this.taskService.createTask(newTask).subscribe(() => {
      this.loadTasks(); // Refresh the task list
    });
  }

  editTask(task: Task): void {
    this.editingTask = { ...task }; // Create a copy for editing
  }

  updateTask(task: Task): void {
    this.taskService.updateTask(task).subscribe(() => {
      this.editingTask = null; // Clear the editing task
      this.loadTasks(); // Refresh the task list
    });
  }

  deleteTask(taskId: number): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(taskId).subscribe(() => {
        this.loadTasks(); // Refresh the task list
      });
    }
  }

  onSignOff(): void {
    this.authService.logout(); // Call your logout method from AuthService
  }
}

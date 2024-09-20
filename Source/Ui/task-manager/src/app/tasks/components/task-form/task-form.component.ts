import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Task } from '../../models/task'; // Assuming you have a Task model

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent {
  @Input() isEditing: boolean = false;
  @Input() taskData: Task = { id: 0, title: '', description: '', completed: false }; // Initialize with default values
  @Output() taskAdded = new EventEmitter<Task>();
  @Output() taskUpdated = new EventEmitter<Task>();
  @Output() editCanceled = new EventEmitter<void>();

  onSubmit(): void {
    if (this.isEditing) {
      this.taskUpdated.emit(this.taskData);
    } else {
      this.taskAdded.emit(this.taskData);
    }
    this.resetForm();
  }

  onCancelEdit(): void {
    this.editCanceled.emit();
    this.resetForm();
  }

  private resetForm(): void {
    this.taskData = { id: 0, title: '', description: '', completed: false };
  }
}

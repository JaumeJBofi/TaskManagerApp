import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TASK_STATUS, UserTaskDto } from '../../models/task';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent {
  @Input() isEditing: boolean = false;
  @Input() taskData: UserTaskDto; 
  @Output() taskAdded = new EventEmitter<UserTaskDto>();
  @Output() taskUpdated = new EventEmitter<UserTaskDto>();
  @Output() editCanceled = new EventEmitter<void>();
  
  dueDateInput: string;

  constructor(private taskService: TaskService){

    this.taskData =  { id: '', title: '', description: '',status: TASK_STATUS.PENDING, dueDate: taskService.calculateDueDate() };
    this.dueDateInput = this.formatDateToInput(this.taskData.dueDate);        
  }

  statusTypes = Object.values(TASK_STATUS).filter(value => typeof value === 'number').map(o=>{ return {value: o, display: this.taskService.displayStatus(o as TASK_STATUS)}});


  private formatDateToInput(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Months are 0-based
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  updateDueDate(event: Event): void {
    this.taskData.dueDate = new Date(String(event)); // Update the dueDate with the new value
  }

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
    this.taskData = { id: '', title: '', description: '',status: TASK_STATUS.PENDING, dueDate: this.taskService.calculateDueDate() };
    this.dueDateInput = this.formatDateToInput(this.taskData.dueDate);
  }
}

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Or ReactiveFormsModule
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskFormComponent } from './components/task-form/task-form.component';
import { TasksRoutingModule } from './tasks-routing.module';

@NgModule({
  declarations: [TaskListComponent, TaskFormComponent],
  imports: [
    CommonModule,
    FormsModule, // Import FormsModule
    TasksRoutingModule
  ]
})
export class TasksModule { }

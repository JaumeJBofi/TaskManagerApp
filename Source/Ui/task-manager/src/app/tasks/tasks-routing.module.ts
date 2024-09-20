import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TaskListComponent } from './components/task-list/task-list.component';
import { AuthGuard } from '../auth/auth.guard'; // Assuming you have an auth guard

const routes: Routes = [
  { 
    path: 'tasks', 
    component: TaskListComponent,
    canActivate: [AuthGuard] // Protect this route 
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TasksRoutingModule { }

import { Routes } from '@angular/router';
import { TaskListComponent } from '../app/tasks/components/task-list/task-list.component';
import { LoginComponent } from './auth/components/login/login.component';
import { SignupComponent } from './auth/components/signup/signup.component';
import { AuthGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // Redirect to login by default
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { 
    path: 'tasks', 
    component: TaskListComponent, 
    canActivate: [AuthGuard] // Protect the tasks route
  }
];

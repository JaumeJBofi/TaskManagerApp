import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',  
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginDto = { userName: '', password: '' };
  error: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.authService.login(this.loginDto).subscribe({
      next: () => {
        this.router.navigate(['/tasks']); // Navigate to tasks on success
      },
      error: (err) => {
        alert("The user couldn't be logged in: " + err?.error?.message);          
      }
    });
  }
}

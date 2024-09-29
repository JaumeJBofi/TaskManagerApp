import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',  
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  signInDto = { 
    userName: '', 
    email: '', 
    password: '' 
  };
  error: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  signup() {
    this.authService.signin(this.signInDto).subscribe({
      next: () => {
        this.router.navigate(['/login']); // Navigate to login on success
      },
      error: (err) => {
        alert("The user couldn't be created: " + err?.error?.message);        
      }
    });
  }
}

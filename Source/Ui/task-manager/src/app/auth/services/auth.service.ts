import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.dev';

interface LoginResponse {
  // Define the structure of the response from your API
  token: string;
  // ... other properties
}

@Injectable({
  providedIn: 'root' 
})
export class AuthService {
  private apiUrl = environment.apiBaseUrl + '/auth'; 
  constructor(private http: HttpClient) { }

  login(credentials: any): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, credentials);
  }

  signup(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/signup`, userData);
  }

  logout(){
    return true;
  }

  isLoggedIn(){
    return true;
  }

  // ... other methods for token handling, logout, etc.
}

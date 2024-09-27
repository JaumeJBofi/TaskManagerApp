import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.dev';

interface LoginResponse {  
  token: string;  
}

@Injectable({
  providedIn: 'root' 
})
export class AuthService {
  headers = new HttpHeaders({
    "Content-Type": "application/json",  
  });

  private apiUrl = environment.apiBaseUrl + '/Auth'; 
  constructor(private http: HttpClient) { }

  login(loginDto: any): Observable<LoginResponse> {
    //response.xsrfToken
    //localStorage.setItem('XSRF-TOKEN', token);
    return this.http.post<LoginResponse>(`${this.apiUrl}/Login`, loginDto, { headers: this.headers });
  }

  signin(signInDto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/SignIn`, JSON.stringify(signInDto),{ headers: this.headers});
  }

  logout(){
    return true;
  }

  isLoggedIn(){
    return true;
  }

  // ... other methods for token handling, logout, etc.
}

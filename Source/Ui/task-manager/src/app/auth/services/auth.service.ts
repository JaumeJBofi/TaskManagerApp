import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment.dev';

interface UserDto {
  userNmae: string,
  email: string
}

interface LoginResponse {  
  xsrfToken: string;  
  accessToken: string;  
  userDto: UserDto
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
    return this.http.post<LoginResponse>(`${this.apiUrl}/Login`, loginDto, { headers: this.headers }).pipe(
      map(response => {
        localStorage.setItem('XSRF-TOKEN', response.xsrfToken);
        localStorage.setItem('ACCESS-TOKEN',response.accessToken);        
        return response;
    }));
  }

  signin(signInDto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/SignIn`, JSON.stringify(signInDto),{ headers: this.headers});
  }

  logout(): Observable<any>{
    return this.http.post(`${this.apiUrl}/Logout`,{});
  }

  isLoggedIn(): Observable<boolean>{
    return this.http.post(`${this.apiUrl}/IsUserLogged`,{}).pipe(
      map(() => true), 
      catchError(() => of(false)) 
    );
  }  
}

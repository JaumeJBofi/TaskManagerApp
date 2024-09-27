import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, Observable } from 'rxjs';
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

  private loggedUser: UserDto | undefined = undefined;
  private apiUrl = environment.apiBaseUrl + '/Auth'; 
  constructor(private http: HttpClient) { }

  login(loginDto: any): Observable<LoginResponse> {      
    return this.http.post<LoginResponse>(`${this.apiUrl}/Login`, loginDto, { headers: this.headers }).pipe(
      map(response => {
        localStorage.setItem('XSRF-TOKEN', response.xsrfToken);
        localStorage.setItem('ACCESS-TOKEN',response.accessToken);
        this.loggedUser = response.userDto;
        return response;
    }));
  }

  signin(signInDto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/SignIn`, JSON.stringify(signInDto),{ headers: this.headers});
  }

  logout(){
    return this.http.post(`${this.apiUrl}/Logout`, JSON.stringify(this.loggedUser),{ headers: this.headers});
  }

  isLoggedIn(){
    return true;
  }  
}

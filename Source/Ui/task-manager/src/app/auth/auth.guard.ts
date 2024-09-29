import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { map, Observable, pipe } from 'rxjs';
import { AuthService } from '../auth/services/auth.service'; // Adjust path if needed

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    // Check if the user is authenticated (e.g., has a valid token)
    return this.authService.isLoggedIn().pipe(map((result: boolean)=>{
      if (result) { 
        return true; // Allow access to the route
      } else {
        // Redirect to the login page or show an error message
        return this.router.parseUrl('/login'); // Redirect to login
      }
    })); 
  }
}

import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor() {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Retrieve the JWT token from localStorage or a service
        const token = localStorage.getItem('ACCESS-TOKEN'); // Adjust according to how you store your token

        // Clone the request and add the authorization header
        if (token) {
            const clonedRequest = req.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
            return next.handle(clonedRequest);
        }

        return next.handle(req); // If no token, pass the request unchanged
    }
}

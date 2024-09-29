import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor() {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Retrieve the JWT token from localStorage or a service
        const authToken = localStorage.getItem('ACCESS-TOKEN'); // Adjust according to how you store your token
        const xsrfToken = localStorage.getItem('XSRF-TOKEN'); // Adjust according to how you store your token

        // Clone the request and add the authorization header
        if (authToken || xsrfToken) {
            const clonedRequest = req.clone({
                setHeaders: {
                    "Authorization": `Bearer ${authToken}`,
                    "X-XSRF-TOKEN" : xsrfToken || ''
                }, withCredentials: true
            });
            return next.handle(clonedRequest);
        }

        return next.handle(req).pipe(
            tap(event => {
                // Check if the event is a HttpResponse
                if (event instanceof HttpResponse) {
                    // Check for a specific header in the response
                    const newToken = event.headers.get('AccessRefresh'); // Adjust the header name

                    if (newToken) {
                        // Save the new token if present
                        localStorage.setItem('ACCESS-TOKEN', newToken);
                    }
                }
            })
        );
    }
}

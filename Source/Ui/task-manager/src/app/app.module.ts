import { BrowserModule } from '@angular/platform-browser';
import { importProvidersFrom, NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { CommonModule } from '@angular/common';
import { AuthModule } from './auth/auth.module';
import { TasksModule } from './tasks/tasks.module';
import { RouterOutlet } from '@angular/router';
import { HTTP_INTERCEPTORS, HttpClientModule, HttpClientXsrfModule } from '@angular/common/http';
import { AuthInterceptor } from './auth/services/auth-interceptor.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,  
    RouterOutlet,
    HttpClientModule,
    AuthModule,
    TasksModule,
    AppRoutingModule
  ],
  providers: [
    importProvidersFrom(HttpClientModule),
    importProvidersFrom(
        HttpClientXsrfModule.withOptions({
        cookieName: 'XSRF-TOKEN',
        headerName: 'X-XSRF-TOKEN',
      })
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true // This allows multiple interceptors
    }
  ], // Services for the root module (usually at least one)
  bootstrap: [AppComponent] // The root component of your application
})
export class AppModule { }

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { CommonModule } from '@angular/common';
import { AuthModule } from './auth/auth.module';
import { TasksModule } from './tasks/tasks.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,  
    AuthModule,
    TasksModule,
    AppRoutingModule
  ],
  providers: [], // Services for the root module (usually at least one)
  bootstrap: [AppComponent] // The root component of your application
})
export class AppModule { }

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent    
  ],
  imports: [
    BrowserModule,    
  ],
  providers: [], // Services for the root module (usually at least one)
  bootstrap: [AppComponent] // The root component of your application
})
export class AppModule { }

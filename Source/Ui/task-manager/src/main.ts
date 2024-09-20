import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module'; // Import AppModule

platformBrowserDynamic().bootstrapModule(AppModule) // Bootstrap AppModule
  .catch(err => console.error(err));
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { AppComponent } from './app.component';
import { DataTableComponent } from './controls/data-table/data-table.component';
import { ContactsComponent } from './sections/contacts/contacts.component';
import { DataService } from './services/data.service';
import { Http, HttpModule } from '@angular/http';

import { HttpClientModule } from '@angular/common/http';
import { UseFilterPipe } from './controls/data-table/pipes/use-filter.pipe';
import { UIErrorHandler } from './common/ui-error-handler';

import { AuthenticationService } from './services/authentication.service';
import { routing } from './app.routing';
import { LoginComponent } from './login/login.component';
import { FormsModule } from '@angular/forms';
import { AuthGuard } from './auth/auth.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from './auth/token.interceptor';
import { JwtInterceptor } from './auth/jwt.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    DataTableComponent,
    ContactsComponent,
    UseFilterPipe,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    HttpModule,
    routing,
    FormsModule
  ],
  providers: [DataService, AuthGuard, AuthenticationService,
    {
      provide: ErrorHandler,
      useClass: UIErrorHandler
    }, {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }, {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }

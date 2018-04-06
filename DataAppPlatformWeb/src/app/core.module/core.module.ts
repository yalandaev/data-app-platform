import { NgModule, ErrorHandler } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from './services/authentication.service';
import { DataService } from './services/data.service';
import { AuthGuard } from './auth/auth.guard';
import { UIErrorHandler } from './shared/ui-error-handler';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TokenInterceptor } from './auth/token.interceptor';
import { JwtInterceptor } from './auth/jwt.interceptor';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    BrowserModule,
    HttpClientModule,
    HttpModule,
    FormsModule
  ],
  exports: [],
  declarations: [],
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
})
export class CoreModule { }

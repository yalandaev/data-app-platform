import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { AppComponent } from './app.component';
import { DataTableComponent } from './controls/data-table/data-table.component';
import { ContactsComponent } from './sections/contacts/contacts.component';
import { Http, HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { UseFilterPipe } from './controls/data-table/pipes/use-filter.pipe';
import { routing } from './app.routing';
import { LoginComponent } from './login/login.component';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ContactPageComponent } from './sections/contacts/contact-page.component';
import { TextInputComponent } from './controls/input/text-input/text-input.component';
import { CoreModule } from './core.module/core.module';
import { LookupInputComponent } from './controls/input/lookup-input/lookup-input.component';
import { SelectInputComponent } from './controls/input/select-input/select-input.component';
import { SelectFilterPipe } from './controls/input/select-input/select-filter-pipe/selectbox-filter-pipe.pipe';
import { QuickFilterComponent } from './controls/quick-filter/quick-filter.component';
import { QuickFilterItemComponent } from './controls/quick-filter-item/quick-filter-item.component';


@NgModule({
  declarations: [
    AppComponent,
    DataTableComponent,
    ContactsComponent,
    ContactPageComponent,
    UseFilterPipe,
    LoginComponent,
    TextInputComponent,
    LookupInputComponent,
    SelectInputComponent,
    SelectFilterPipe,
    QuickFilterComponent,
    QuickFilterItemComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    HttpModule,
    routing,
    FormsModule,
    CoreModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

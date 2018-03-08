import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import { DataTableComponent } from './controls/data-table/data-table.component';
import { ContactsComponent } from './sections/contacts/contacts.component';
import { DataService } from './services/data.service';
import { Http } from '@angular/http';
import { HttpModule } from '@angular/http/src/http_module';
import { HttpClientModule } from '@angular/common/http';
import { UseFilterPipe } from './controls/data-table/pipes/use-filter.pipe';


@NgModule({
  declarations: [
    AppComponent,
    DataTableComponent,
    ContactsComponent,
    UseFilterPipe
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [DataService],
  bootstrap: [AppComponent]
})
export class AppModule { }

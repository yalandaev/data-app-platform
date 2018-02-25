import { Component, OnInit } from '@angular/core';
import { DataTableColumn } from '../../controls/data-table/models/column.model';
import { ColumnType } from '../../controls/data-table/models/columnType';

@Component({
  selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  styleUrls: ['./contacts.component.css']
})
export class ContactsComponent implements OnInit {

  columns: Array<DataTableColumn> = null;
  data = null;

  constructor() {
    this.columns = [
      {
        name: 'Name',
        displayName: 'Name',
        type: ColumnType.Text,
        width: 10
      },
      {
        name: 'BirthDate',
        displayName: 'Birth date',
        type: ColumnType.DateTime,
        width: 15
      }];
  }

  ngOnInit() {
  }
}

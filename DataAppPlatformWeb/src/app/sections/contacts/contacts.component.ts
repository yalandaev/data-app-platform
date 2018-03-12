import { Component, OnInit } from '@angular/core';
import { DataTableColumn } from '../../controls/data-table/models/column.model';
import { ColumnType } from '../../controls/data-table/models/columnType';
import { FilterGroup } from '../../controls/data-table/models/filter/filterGroup.model';
import { LogicalOperation } from '../../controls/data-table/models/filter/logicalOperation.model';
import { ComparisonType } from '../../controls/data-table/models/filter/comparisonType.model';
import { Condition } from '../../controls/data-table/models/filter/condition.model';

@Component({
  selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  styleUrls: ['./contacts.component.css']
})
export class ContactsComponent implements OnInit {

  columns: Array<DataTableColumn> = null;
  filter: FilterGroup = null;
  data = null;

  constructor() {
    this.columns = [
      {
        name: 'Id',
        displayName: 'Id',
        type: ColumnType.Text,
        width: 10,
      },
      {
        name: 'FirstName',
        displayName: 'First Name',
        type: ColumnType.Text,
        width: 10
      },
      {
        name: 'LastName',
        displayName: 'Last Name',
        type: ColumnType.Text,
        width: 10
      },
      {
        name: 'Email',
        displayName: 'Its email!',
        type: ColumnType.Text,
        width: 10
      },
      {
        name: 'BirthDate',
        displayName: 'Birth date',
        type: ColumnType.DateTime,
        width: 15,
        formatter: 'date:format'
      },
      {
        name: 'Manager.FirstName',
        displayName: 'Manager',
        type: ColumnType.Text,
        width: 15,
      },
      {
        name: 'Manager.Manager.FirstName',
        displayName: 'Manager of manager',
        type: ColumnType.Text,
        width: 15,
      }];

      this.filter = new FilterGroup(
        LogicalOperation.AND,
        [
          new Condition('Manager.Manager.FirstName', ComparisonType.Equals, 'Manager.Manager.FirstName'),
          new Condition('Manager.FirstName', ComparisonType.Equals, 'Manager.FirstName')
        ],
        [
          new FilterGroup(
            LogicalOperation.OR,
            [
              new Condition('FirstName', ComparisonType.Equals, 'FirstName'),
              new Condition('LastName', ComparisonType.Equals, 'LastName')
            ], [])
        ]);
    }

  ngOnInit() {
  }
}

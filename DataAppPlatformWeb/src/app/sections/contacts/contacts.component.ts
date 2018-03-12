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
        displayName: 'Email',
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
        name: 'Manager.BirthDate',
        displayName: 'Manager birthdate',
        type: ColumnType.Text,
        width: 15,
        formatter: 'date:format'
      }];

      this.filter = new FilterGroup(
        LogicalOperation.AND,
        [
          new Condition('FirstName', ComparisonType.FilledIn),
          new Condition('Manager.FirstName', ComparisonType.Equals, 'Mark'),
          new Condition('LastName', ComparisonType.StartWith, 'Yalandaev')
        ],
        [
          new FilterGroup(
            LogicalOperation.OR,
            [
              new Condition('Manager.Phone', ComparisonType.Equals, '79171573840'),
              new Condition('Manager.Email', ComparisonType.Equals, 'ivanov@gmail.com')
            ], [])
        ]);
    }

  ngOnInit() {
  }
}

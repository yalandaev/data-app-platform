import { Component, OnInit } from '@angular/core';
import { DataTableColumn } from '../../controls/data-table/models/column.model';
import { ColumnType } from '../../controls/data-table/models/column-type.enum';
import { FilterGroup, Condition, LogicalOperation, ComparisonType, ConditionType } from '../../core.module';


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
          new Condition('Manager.FirstName', ComparisonType.Equals, ConditionType.Constant, 'Mark'),
          new Condition('LastName', ComparisonType.StartWith, ConditionType.Constant, 'Yalandaev')
        ],
        [
          new FilterGroup(
            LogicalOperation.OR,
            [
              new Condition('Manager.Phone', ComparisonType.Equals, ConditionType.Constant, '79171573840'),
              new Condition('Manager.Email', ComparisonType.Equals, ConditionType.Constant, 'ivanov@gmail.com')
            ], [])
        ]);
    }

  ngOnInit() {
  }
}

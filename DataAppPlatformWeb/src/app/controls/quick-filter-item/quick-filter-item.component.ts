import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Condition } from '../../core.module';
import { ComparisonType } from '../../core.module/api/data-service/filter/comparison-type.enum';
import { EntityPropertySchema } from '../edit-page/entity-property-schema';

@Component({
  selector: 'app-quick-filter-item',
  templateUrl: './quick-filter-item.component.html',
  styleUrls: ['./quick-filter-item.component.css']
})
export class QuickFilterItemComponent implements OnInit {

  columnNames = [
    {
        value: 'FirstName',
        name: 'First name'
    },
    {
        value: 'Manager.FirstName',
        name: 'Manager'
    },
    {
        value: 'LastName',
        name: 'Last Name'
    },
    {
        value: 'Age',
        name: 'Age'
    }
  ];

  comparisonTypes = [
    {
        value: ComparisonType.Contains,
        name: 'Contains'
    },
    {
      value: ComparisonType.EndWith,
      name: 'End with'
    },
    {
      value: ComparisonType.Equals,
      name: 'Equals'
    },
    {
      value: ComparisonType.FilledIn,
      name: 'Filled in'
    },
    {
      value: ComparisonType.Less,
      name: 'Less'
    },
    {
      value: ComparisonType.LessOrEquals,
      name: 'Less or equals'
    },
    {
      value: ComparisonType.More,
      name: 'More'
    },
    {
      value: ComparisonType.MoreOrEquals,
      name: 'More or equals'
    },
    {
      value: ComparisonType.NotContains,
      name: 'Not contains'
    },
    {
      value: ComparisonType.NotEquals,
      name: 'Not equals'
    },
    {
      value: ComparisonType.NotFilledIn,
      name: 'Not filled in'
    },
    {
      value: ComparisonType.StartWith,
      name: 'Start with'
    }
  ];

  value: EntityPropertySchema = {
    label: 'Value',
    required: true,
    visible: true,
    enabled: true
};

  constructor() { }

  @Input()
  model: Condition;

  @Output()
  modelChange: EventEmitter<Condition> = new EventEmitter<Condition>();

  ngOnInit() {
  }

}

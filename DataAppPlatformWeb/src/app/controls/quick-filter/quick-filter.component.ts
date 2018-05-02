import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FilterGroup, Condition } from '../../core.module';

@Component({
  selector: 'app-quick-filter',
  templateUrl: './quick-filter.component.html',
  styleUrls: ['./quick-filter.component.css']
})
export class QuickFilterComponent implements OnInit {

  constructor() { }

  @Input()
  model: FilterGroup;

  @Output()
  modelChange: EventEmitter<FilterGroup> = new EventEmitter<FilterGroup>();

  ngOnInit() {
  }

  addCondition() {
    this.model.addCondition(new Condition(null, null));
  }

}

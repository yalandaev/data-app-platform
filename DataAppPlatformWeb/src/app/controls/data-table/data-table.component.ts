import { Component, OnInit, Input } from '@angular/core';
import { DataTableColumn } from './models/column.model';
import { DataService } from '../../services/data.service';
import { DataRequest } from './models/dataRequest.model';
import { FilterGroup } from './models/filter/filterGroup.model';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.css']
})
export class DataTableComponent implements OnInit {

  data = [];
  currentPage = 1;

  constructor(private dataService: DataService) {

  }

  @Input()
  entitySchema: string;

  @Input()
  columns: Array<DataTableColumn>;

  @Input()
  pageSize: number;

  @Input()
  filter: FilterGroup;

  ngOnInit() {
    this.dataService.getData(new DataRequest(this.entitySchema, this.columns, this.filter, this.currentPage, this.pageSize))
    .subscribe(data => {
      this.data = data.Data;
    });
  }
}

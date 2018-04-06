import { Component, OnInit, Input } from '@angular/core';
import { DataTableColumn } from './models/column.model';
import { Router } from '@angular/router';
import { DataRequest, FilterGroup, Sort, DataService } from '../../core.module';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.css']
})
export class DataTableComponent implements OnInit {

  data = [];
  currentPage = 1;

  constructor(
    private dataService: DataService,
    private router: Router) {
  }

  @Input()
  entitySchema: string;

  @Input()
  orderBy: string;

  @Input()
  sort: Sort;

  @Input()
  columns: Array<DataTableColumn>;

  @Input()
  pageSize: number;

  @Input()
  filter: FilterGroup;

  debugInformation: string;

  ngOnInit() {
    this.dataService.getData(
      new DataRequest(this.entitySchema,
        this.orderBy,
        this.sort,
        this.columns.map(f => f.name),
        this.filter,
        this.currentPage,
        this.pageSize))
    .subscribe(data => {
      this.data = data.Data;
      this.debugInformation = data.DebugInformation;
    });
  }

  getFilter(column: DataTableColumn) {
    return column.formatter;
  }

  onRowClick(item: any) {
    this.router.navigate(['/contacts/', item.Id]);
  }
}

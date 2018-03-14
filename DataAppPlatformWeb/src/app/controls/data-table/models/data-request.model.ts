import { DataTableColumn } from './column.model';
import { FilterGroup } from './filter/filter-group.model';
import { Sort } from './filter/sort.enum';
export class DataRequest {
    constructor(
        private entitySchema: string,
        private orderBy: string,
        private sort: Sort,
        private columns: Array<DataTableColumn>,
        private filter: FilterGroup,
        private page: number,
        private pageSize: number) {}
}

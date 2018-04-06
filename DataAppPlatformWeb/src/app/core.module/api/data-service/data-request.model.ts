import { Sort } from './filter/sort.enum';
import { DataColumn } from './data-column.model';
import { FilterGroup } from './filter/filter-group.model';

export class DataRequest {
    constructor(
        private entitySchema: string,
        private orderBy: string,
        private sort: Sort,
        private columns: Array<string>,
        private filter: FilterGroup,
        private page: number,
        private pageSize: number) {}
}

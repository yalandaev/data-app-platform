import { DataTableColumn } from './column.model';
import { FilterGroup } from './filter/filterGroup.model';
export class DataRequest {
    constructor(
        private entitySchema: string,
        private columns: Array<DataTableColumn>,
        private filter: FilterGroup,
        private page: number,
        private pageSize: number) {}
}

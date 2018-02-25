import { DataTableColumn } from './column.model';
export class DataRequest {
    constructor(
        private entitySchema: string,
        private columns: Array<DataTableColumn>,
        private filter: {},
        private page: number,
        private pageSize: number) {}
}

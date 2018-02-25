import { DataTableColumn } from './column.model';
export class DataRequest {
    constructor(
        entitySchema: string,
        columns: Array<DataTableColumn>,
        filter: {},
        page: number,
        pageSize: number) {}
}

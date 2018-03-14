import { ColumnType } from './column-type.enum';

export class DataTableColumn {
    public name: string;
    public displayName: string;
    public type: ColumnType;
    public width: number;
    public formatter?: string;
}

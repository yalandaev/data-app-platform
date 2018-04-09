import { FilterGroup } from '../../core.module/api/data-service/filter/filter-group.model';

export class EntityPropertySchema {
    value?: any;
    oldValue?: any;
    label: string;
    enabled: boolean;
    visible: boolean;
    required: boolean;
    displayValue?: string;
    schemaName?: string;
    allowHyperlink?: boolean;
    filter?: FilterGroup;
}

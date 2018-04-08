import { FilterGroup } from './filter/filter-group.model';

export class LookupAutoCompleteRequest {
    constructor(
        private entitySchema: string,
        private term: string,
        private filter: FilterGroup) {}
}

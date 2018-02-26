import { LogicalOperation } from "./logicalOperation.model";
import { Condition } from "./condition.model";

export class FilterGroup {
    constructor(
        private logicalOperation: LogicalOperation,
        private conditions: Array<Condition>,
        private filterGroups: Array<FilterGroup>
    ) {}
}
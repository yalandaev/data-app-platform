import { LogicalOperation } from './logical-operation.enum';
import { Condition } from './condition.model';

export class FilterGroup {
    constructor(
        private logicalOperation: LogicalOperation,
        private conditions: Array<Condition>,
        private filterGroups?: Array<FilterGroup>
    ) {}

    addCondition(condition: Condition) {
        this.conditions.push(condition);
    }
}

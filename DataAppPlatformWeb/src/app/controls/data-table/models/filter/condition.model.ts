import { ComparisonType } from './comparison-type.enum';
import { ConditionType } from './condition-type.enum';


export class Condition {
    constructor(
        private column: string,
        private comparisonType: ComparisonType,
        private type: ConditionType = ConditionType.Constant,
        private value?: any
    ) {}
}

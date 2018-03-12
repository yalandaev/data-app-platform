import { ComparisonType } from "./comparisonType.model";

export class Condition {
    constructor(
        private column: string,
        private comparisonType: ComparisonType,
        private value?: any
    ) {}
}

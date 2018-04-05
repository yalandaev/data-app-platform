import { EntityPropertySchema } from '../common/entity-property-schema';
import { IDictionary } from '../../common/IDictionary.model';

export class PageViewModel {
    constructor(
        public entitySchema: string,
        public fields: IDictionary<EntityPropertySchema>,
    ) {}
    public entityId: number;
}

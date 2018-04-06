import { IDictionary } from '../../core.module/shared';
import { EntityPropertySchema } from './entity-property-schema';


export class PageViewModel {
    constructor(
        public entitySchema: string,
        public fields: IDictionary<EntityPropertySchema>,
    ) {}
    public entityId: number;
}

import { IDictionary } from '../../common/IDictionary.model';
import { EntityDataFieldUpdate } from './entity-data-field-update.model';

export class EntityDataUpdateRequest {
    constructor(
        public entitySchema: string,
        public fields: IDictionary<EntityDataFieldUpdate>,
        public entityId: number
    ) {}
}

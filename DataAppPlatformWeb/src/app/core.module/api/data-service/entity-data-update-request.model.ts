import { EntityDataFieldUpdate } from './entity-data-field-update.model';
import { IDictionary } from '../../shared/IDictionary.model';

export class EntityDataUpdateRequest {
    constructor(
        public entitySchema: string,
        public fields: IDictionary<EntityDataFieldUpdate>,
        public entityId: number
    ) {}
}

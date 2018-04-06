export class EntityDataQueryRequest {
    constructor(
        private entitySchema: string,
        private entityId: number,
        private columns: Array<string>) {}
}


export class EntityDataRequest {
    constructor(
        private entitySchema: string,
        private entityId: number,
        private columns: Array<string>) {}
}


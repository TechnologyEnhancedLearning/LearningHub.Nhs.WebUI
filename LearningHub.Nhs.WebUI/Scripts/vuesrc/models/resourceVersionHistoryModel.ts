export class ResourceVersionHistoryModel {
    id: number;

    public constructor(init?: Partial<ResourceVersionHistoryModel>) {
        Object.assign(this, init);
    }
}
export class ActiveContentModel {
    resourceId: number;
    resourceVersionId: number;

    public constructor(init?: Partial<ActiveContentModel>) {
        Object.assign(this, init);
    }
}
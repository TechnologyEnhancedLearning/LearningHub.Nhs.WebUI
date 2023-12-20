export class ResourceHeaderModel {
    id: number;
    title: string;

    constructor(init?: Partial<ResourceHeaderModel>) {
        Object.assign(this, init);
    }
}
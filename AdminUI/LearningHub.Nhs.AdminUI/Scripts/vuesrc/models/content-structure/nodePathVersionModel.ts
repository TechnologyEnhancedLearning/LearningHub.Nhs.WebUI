export class NodePathVersionModel {
    name: string;
    dirty: boolean; 

    public constructor(init?: Partial<NodePathVersionModel>) {
        Object.assign(this, init);
    }
}
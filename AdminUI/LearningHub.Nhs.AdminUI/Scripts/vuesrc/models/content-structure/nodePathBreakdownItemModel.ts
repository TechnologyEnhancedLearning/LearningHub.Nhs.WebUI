export class NodePathBreakdownItemModel {
    NodeId: number;
    NodepathId: number;
    Depth: number;
    NodeName: string;

    public constructor(init?: Partial<NodePathBreakdownItemModel>) {
        Object.assign(this, init);
    }
}

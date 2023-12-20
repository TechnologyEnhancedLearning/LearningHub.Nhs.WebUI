export class NodePathModel {
    id: number;
    nodeId: number;
    nodePathString: string;
    catalogueNodeId: number;
    IsActive: boolean;

    public constructor(init?: Partial<NodePathModel>) {
        Object.assign(this, init);
    }
}
import { NodeContentAdminModel } from "./nodeContentAdminModel";

export class FolderNodeModel {
    hierarchyEditId: number;
    hierarchyEditDetailId: number;
    nodeId: number;
    nodeVersionId: number;
    name: string;
    description: string;
    parentNodeId: number;
    path: string;
    parentNode: NodeContentAdminModel;
    public constructor(init?: Partial<FolderNodeModel>) {
        Object.assign(this, init);
    }
}
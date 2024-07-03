import { NodeContentAdminModel } from "./nodeContentAdminModel";
import { NodePathBreakdownModel } from "./NodePathBreakdownModel";
export class FolderNodeModel {
    hierarchyEditId: number;
    hierarchyEditDetailId: number;
    nodeId: number;
    nodeVersionId: number;
    name: string;
    description: string;
    parentNodeId: number;
    parentNodePathId: number;
    path: string;
    parentNode: NodeContentAdminModel;
    nodePaths: NodePathBreakdownModel[];
    public constructor(init?: Partial<FolderNodeModel>) {
        Object.assign(this, init);
    }
}
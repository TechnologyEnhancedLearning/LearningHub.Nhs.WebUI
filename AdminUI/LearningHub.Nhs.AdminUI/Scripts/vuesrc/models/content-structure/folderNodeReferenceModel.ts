import { NodeContentAdminModel } from "./NodeContentAdminModel";
import { NodePathBreakdownModel } from "./NodePathBreakdownModel";

export class FolderNodeReferenceModel {
    hierarchyEditId: number;
    hierarchyEditDetailId: number;
    nodeId: number;
    name: string;
    path: string;
    nodePaths: NodePathBreakdownModel[];
    parentNode: NodeContentAdminModel;
    public constructor(init?: Partial<FolderNodeReferenceModel>) {
        Object.assign(this, init);
    }
}
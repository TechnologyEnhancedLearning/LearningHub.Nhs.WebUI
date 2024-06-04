import { NodeContentAdminModel } from "./NodeContentAdminModel";
import { NodePathBreakdownModel } from "./NodePathBreakdownModel";

export class NodePathDisplayVersionModel {
    hierarchyEditId: number;
    hierarchyEditDetailId: number;
    nodePathId: number;
    nodePathDisplayVersionId: number;
    name: string;
    path: string;
    nodePaths: NodePathBreakdownModel[];
/*    parentNode: NodeContentAdminModel;*/
    public constructor(init?: Partial<NodePathDisplayVersionModel>) {
        Object.assign(this, init);
    }
}
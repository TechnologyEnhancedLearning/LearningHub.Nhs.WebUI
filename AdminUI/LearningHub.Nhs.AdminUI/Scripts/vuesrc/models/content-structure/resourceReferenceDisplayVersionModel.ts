import { NodePathBreakdownModel } from "./NodePathBreakdownModel";

export class ResourceReferenceDisplayVersionModel {
    hierarchyEditId: number;
    hierarchyEditDetailId: number;
    resourceReferenceId: number;
    resourceReferenceDisplayVersionId: number;
    name: string;
    path: string;
    nodePaths: NodePathBreakdownModel[];
    public constructor(init?: Partial<ResourceReferenceDisplayVersionModel>) {
        Object.assign(this, init);
    }
}
import { NodePathBreakdownItemModel } from "./nodePathBreakdownItemModel";

export class NodePathBreakdownModel {
    NodePathBreakdown: NodePathBreakdownItemModel[];
    public constructor(init?: Partial<NodePathBreakdownModel>) {
        Object.assign(this, init);
    }
}

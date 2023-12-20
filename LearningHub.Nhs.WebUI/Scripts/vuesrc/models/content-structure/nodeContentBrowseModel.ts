import { RatingSummaryBasicModel } from "../ratingSummaryBasicModel";

export class NodeContentBrowseModel {
    name: string;
    description: string;
    nodeTypeId: number;
    nodeId: number;
    nodeVersionId: number;
    resourceId: number;
    resourceVersionId: number;
    resourceReferenceId: number;
    resourceTypeId: number;
    versionStatusId: number;
    draftResourceVersionId: number;
    authoredBy: string;
    displayOrder: number;
    durationInMilliseconds: number;
    inEdit: boolean;
    showInTreeView: boolean;
    depth: number;
    childrenLoaded: boolean;
    parent: NodeContentBrowseModel;
    children: NodeContentBrowseModel[];
    path: string;
    ratingSummaryBasicViewModel: RatingSummaryBasicModel;

    public constructor(init?: Partial<NodeContentBrowseModel>) {
        Object.assign(this, init);
        this.children = new Array<NodeContentBrowseModel>();
    }
}
﻿
export class NodeContentAdminModel {
    hierarchyEditDetailId: number;
    name: string;
    description: string;
    nodeTypeId: number;
    nodeId: number;
    nodeVersionId: number;
    hasResourcesInd: boolean;
    hasResourcesInBranchInd: boolean;
    resourceId: number;
    resourceVersionId: number;
    resourceReferenceId: number;
    resourceTypeId: number;
    versionStatusId: number;
    unpublishedByAdmin: boolean;
    resourceInEdit: boolean;
    draftResourceVersionId: number;
    authoredBy: string;
    displayOrder: number;
    durationInMilliseconds: number;
    inEdit: boolean;
    showInTreeView: boolean;
    depth: number;
    childrenLoaded: boolean;
    parent: NodeContentAdminModel;
    children: NodeContentAdminModel[];
    path: string;

    public constructor(init?: Partial<NodeContentAdminModel>) {
        Object.assign(this, init);
        this.children = new Array<NodeContentAdminModel>();
    }
}
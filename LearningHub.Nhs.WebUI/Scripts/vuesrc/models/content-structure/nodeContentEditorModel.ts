export class NodeContentEditorModel {
    name: string;
    description: string;
    nodeTypeId: number;
    nodeId: number;
    nodePathId: number;
    nodeVersionId: number;
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
    parent: NodeContentEditorModel;
    children: NodeContentEditorModel[];
    path: string;

    public constructor(init?: Partial<NodeContentEditorModel>) {
        Object.assign(this, init);
        this.children = new Array<NodeContentEditorModel>();
    }
}
export class HierarchyEditModel {
    id: number;
    nodeId: number;
    rootHierarchyEditDetailId: number;
    nodeName: string;
    userId: number;
    userName: string;
    lastPublishedDate: Date;
    createDate: Date;
    hierarchyEditStatus: HierarchyEditStatusEnum;
    public constructor(init?: Partial<HierarchyEditModel>) {
        Object.assign(this, init);
    }
}

export enum HierarchyEditStatusEnum {
    Draft = 1,
    Published = 2,
    Discarded = 3,
    Publishing = 4,
    SubmittedToPublishingQueue = 5,
    FailedToPublish = 6,
}
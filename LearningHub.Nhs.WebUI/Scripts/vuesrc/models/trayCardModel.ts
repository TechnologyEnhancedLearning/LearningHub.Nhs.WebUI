import { ResourceType } from '../constants'
import { VersionStatus } from '../constants'

export class TrayCardModel {
    id: number;
    resourceId: number;
    resourceVersionId: number;
    resourceReferenceId: number;
    versionStatusEnum: VersionStatus;
    previousPublishedStatusEnum: VersionStatus;
    versionStatusDescription: string;
    resourceTypeEnum: ResourceType;
    resourceTypeDescription: string;
    backgroundImage: string;    
    title: string;
    version: string;
    actionRequired: boolean;
    draftCategory: boolean;
    publishedCategory: boolean;
    unpublishedCategory: boolean;
    inEdit: boolean;
    isFlagged: boolean;
    flaggedBy: string;
    flaggedByAdmin: boolean;
    flaggedDate: Date;
    createdBy: string;
    createdDate: Date;
    publishedBy: string;
    publishedDate: Date;
    unpublishedBy: string;
    unpublishedByAdmin: boolean;
    unpublishedDate: Date;
    contributedOn: string;
    readOnly: boolean;
    time: string;
    constructor(init?: Partial<TrayCardModel>) {
        Object.assign(this, init);
    }
}
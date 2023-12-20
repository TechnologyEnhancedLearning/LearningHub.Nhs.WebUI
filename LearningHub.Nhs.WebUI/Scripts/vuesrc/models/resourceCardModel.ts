import { VersionStatus } from '../constants'

export class ResourceCardModel {
    contributedOn: string;
    unpublishedDate: Date;
    unpublishedBy: string;
    unpublishedByAdmin: boolean;
    createdDate: Date;
    createdBy: string;
    flaggedDate: Date;
    flaggedBy: string;
    readOnly: boolean;
    flaggedByAdmin: boolean;
    inEdit: boolean;
    previousPublishedStatusEnum: VersionStatus;
    unpublishedCategory: boolean;
    publishedCategory: boolean;
    draftCategory: boolean;
    actionRequired: boolean;
    backgroundImage: string;
    id: number;
    isFlagged: boolean;
    resourceReferenceId: number;
    constructor(init?: Partial<ResourceCardModel>) {
        Object.assign(this, init);
    }
}
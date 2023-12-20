import { ResourceType } from '../../constants'
import { VersionStatus } from '../../constants'

export class MyContributionsCardModel {
    resourceId: number;
    resourceVersionId: number;
    resourceReferenceId: number;
    versionStatusEnum: VersionStatus;
    versionStatusDescription: string;
    resourceTypeEnum: ResourceType;
    resourceTypeDescription: string;
    title: string;
    actionRequired: boolean;
    draftCategory: boolean;
    publishedCategory: boolean;
    unpublishedCategory: boolean;
    inEdit: boolean;
    draftResourceVersionId: number;
    isFlagged: boolean;
    flaggedByAdmin: boolean;
    unpublishedByAdmin: boolean;
    contributedOn: string;
    readOnly: boolean;
    time: string;
    hasValidationErrors: boolean;
    constructor(init?: Partial<MyContributionsCardModel>) {
        Object.assign(this, init);
    }
}
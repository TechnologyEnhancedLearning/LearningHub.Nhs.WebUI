import { AssessmentDetailsModel } from "./assessmentDetailsModel";
import { ActivityStatus, ResourceType } from '../../constants';

export class ActivityDetailedItemModel {
    title: string;
    resourceId: number;
    resourceReferenceId: number;
    assessmentResourceActivityId: number;
    majorVersion: number;
    minorVersion: number;
    version: string;
    resourceType: ResourceType;
    activityDate: Date;
    activityDurationSeconds: number;
    resourceDurationMilliseconds: number;
    complete: boolean;
    completionPercentage: number;
    scorePercentage: number;
    isMostRecent: boolean;
    isCurrentResourceVersion: boolean;
    activityStatus: ActivityStatus;
    score: number;
    assessmentDetails: AssessmentDetailsModel;

    public constructor(init?: Partial<ActivityDetailedItemModel>) {
        Object.assign(this, init);
    }
}
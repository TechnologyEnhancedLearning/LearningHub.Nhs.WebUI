import { ActivityDetailedItemModel } from './activityDetailedItemModel';

export class ActivityDetailedModel {
    totalCount: number;
    activities: ActivityDetailedItemModel[];

    public constructor(init?: Partial<ActivityDetailedModel>) {
        Object.assign(this, init);
    }
}
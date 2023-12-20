
export class RatingSummaryBasicModel {
    entityVersionId: number;
    averageRating: number;
    ratingCount: number;
    rating1StarPercent: number;
    rating2StarPercent: number;
    rating3StarPercent: number;
    rating4StarPercent: number;
    rating5StarPercent: number;

    public constructor(init?: Partial<RatingSummaryBasicModel>) {
        Object.assign(this, init);
    }
}

export class RatingSummaryModel {
    entityVersionId: number;
    averageRating: number;
    ratingCount: number;
    rating1StarPercent: number;
    rating2StarPercent: number;
    rating3StarPercent: number;
    rating4StarPercent: number;
    rating5StarPercent: number;
    userRating: number;
    userIsContributor: boolean;
    userCanRate: boolean;
    userHasAlreadyRated: boolean;

    public constructor(init?: Partial<RatingSummaryModel>) {
        Object.assign(this, init);
    }
}
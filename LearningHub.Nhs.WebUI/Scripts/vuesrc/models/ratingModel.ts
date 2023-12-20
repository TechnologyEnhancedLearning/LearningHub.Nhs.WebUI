
export class RatingModel {
    entityVersionId: number;
    rating: number;

    public constructor(init?: Partial<RatingModel>) {
        Object.assign(this, init);
    }
}
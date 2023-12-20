export class RestrictedCatalogueSummaryModel {

    userCount: number;
    accessRequestCount: number;

    public constructor(init?: Partial<RestrictedCatalogueSummaryModel>) {
        Object.assign(this, init);
    }
}
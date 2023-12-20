export class RestrictedCatalogueAccessRequestFilterModel {

    includePending: boolean;
    includeApproved: boolean;
    includeDenied: boolean;

    public constructor(init?: Partial<RestrictedCatalogueAccessRequestFilterModel>) {
        Object.assign(this, init);
    }
}
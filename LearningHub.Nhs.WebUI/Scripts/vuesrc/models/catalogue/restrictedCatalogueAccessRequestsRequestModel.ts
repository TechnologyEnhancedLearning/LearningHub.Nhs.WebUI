export class RestrictedCatalogueAccessRequestsRequestModel {

    catalogueNodeId: number;

    includeNew: boolean;
    includeApproved: boolean;
    includeDenied: boolean;

    public constructor(init?: Partial<RestrictedCatalogueAccessRequestsRequestModel>) {
        Object.assign(this, init);
    }
}
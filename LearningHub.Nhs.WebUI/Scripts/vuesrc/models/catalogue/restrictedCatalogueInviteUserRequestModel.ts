export class RestrictedCatalogueInviteUserRequestModel {

    catalogueNodeId: number;
    emailAddress: string;

    public constructor(init?: Partial<RestrictedCatalogueInviteUserRequestModel>) {
        Object.assign(this, init);
    }
}
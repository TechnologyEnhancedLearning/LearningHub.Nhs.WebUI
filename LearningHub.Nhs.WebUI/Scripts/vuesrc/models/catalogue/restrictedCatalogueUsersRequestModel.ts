export class RestrictedCatalogueUsersRequestModel {

    catalogueNodeId: number;
    emailAddressFilter: string;
    includeCatalogueAdmins: boolean;
    includePlatformAdmins: boolean;

    skip: number;
    take: number;

    public constructor(init?: Partial<RestrictedCatalogueUsersRequestModel>) {
        Object.assign(this, init);
    }
}
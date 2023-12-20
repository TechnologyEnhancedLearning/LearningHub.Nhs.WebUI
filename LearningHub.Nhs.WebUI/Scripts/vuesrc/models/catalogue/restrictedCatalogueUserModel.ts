export class RestrictedCatalogueUserModel {

    userUserGroupId: number;
    emailAddress: string;
    fullName: string;
    addedByUsername: string;
    addedDatetime: Date;
    canRemove: boolean;
    showExpandedDetails: boolean;

    public constructor(init?: Partial<RestrictedCatalogueUserModel>) {
        Object.assign(this, init);
    }
}
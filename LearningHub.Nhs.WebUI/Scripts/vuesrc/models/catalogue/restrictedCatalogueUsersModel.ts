import { RestrictedCatalogueUserModel } from './restrictedCatalogueUserModel';

export class RestrictedCatalogueUsersModel {

    userCount: number;
    restrictedCatalogueUsers: RestrictedCatalogueUserModel[];

    public constructor(init?: Partial<RestrictedCatalogueUsersModel>) {
        Object.assign(this, init);
    }
}
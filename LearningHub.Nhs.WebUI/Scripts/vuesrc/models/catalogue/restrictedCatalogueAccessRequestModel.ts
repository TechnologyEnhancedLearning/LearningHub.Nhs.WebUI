import { AccessRequestStatus } from "../../constants";
export class RestrictedCatalogueAccessRequestModel {

    catalogueAccessRequestId: number;
    emailAddress: string;
    fullName: string;
    requestedDatetime: Date;
    catalogueAccessRequestStatus: AccessRequestStatus;
    showExpandedDetails: boolean;
    roleId: number

    public constructor(init?: Partial<RestrictedCatalogueAccessRequestModel>) {
        Object.assign(this, init);
    }
}
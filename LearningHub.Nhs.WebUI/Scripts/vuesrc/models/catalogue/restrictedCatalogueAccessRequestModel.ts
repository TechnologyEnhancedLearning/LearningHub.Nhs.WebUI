import { AccessRequestStatus } from "../../constants";
export class RestrictedCatalogueAccessRequestModel {

    catalogueAccessRequestId: number;
    emailAddress: string;
    fullName: string;
    requestedDatetime: Date;
    catalogueAccessRequestStatus: AccessRequestStatus;
    showExpandedDetails: boolean;

    public constructor(init?: Partial<RestrictedCatalogueAccessRequestModel>) {
        Object.assign(this, init);
    }
}
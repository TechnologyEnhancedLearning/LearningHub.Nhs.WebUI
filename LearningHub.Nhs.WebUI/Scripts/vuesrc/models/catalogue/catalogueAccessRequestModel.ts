import { AccessRequestStatus } from "../../constants";
export class CatalogueAccessRequestModel {

    id: number;
    username: string;
    userFullName: string;
    status: AccessRequestStatus;
    dateRequested: Date;
    dateApproved: Date;
    dateRejected: Date;
    emailAddress: string;
    message: string;
    responseMessage: string;

    public constructor(init?: Partial<CatalogueAccessRequestModel>) {
        Object.assign(this, init);
    }
}
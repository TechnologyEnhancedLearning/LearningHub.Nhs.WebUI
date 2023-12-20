import { FlagModel } from './flagModel';

export class ResourceInformationModel {
    id: number;
    resourceId: number;
    resourceVersionId: number;
    resourceTypeDescription: string;
    title: string;
    description: string;
    additionalInformation: string;  
    version: string;
    keywords: string[];
    nextReviewDate: Date;
    licenseName: string;
    licenseUrl: string;
    resourceFree: boolean;
    cost: number;
    flags: FlagModel[];

    constructor(init?: Partial<ResourceInformationModel>) {
        Object.assign(this, init);
    }
}
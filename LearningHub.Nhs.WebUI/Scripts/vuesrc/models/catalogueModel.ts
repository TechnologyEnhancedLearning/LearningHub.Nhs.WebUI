import { ResourceType } from "../constants";


export class CatalogueBasicModel {
    id: number;
    nodeId: number;
    name: string;
    hidden: boolean;
    url: string;
    badgeUrl: string;
    restrictedAccess: boolean;
    public constructor(init?: Partial<CatalogueBasicModel>) {
        Object.assign(this, init);
    }
}

export class CatalogueModel extends CatalogueBasicModel {
    bannerUrl: string;
    description: string;
    ownerName: string;
    ownerEmailAddress: string;
    resourceOrder: ResourceOrderEnum;
    bookmarkId: number;
    isBookmarked: boolean;
    public constructor(init?: Partial<CatalogueModel>) {
        super(init);
        Object.assign(this, init);
    }
}

export class CatalogueResourceModel {
    resourceId: number;
    resourceVersionId: number;    
    resourceReferenceId: number;
    type: ResourceType;    
    title: string;
    description: string;
    authoredBy: string;
    organisation: string;   
    createdOn: string;
    authoredDateText: string;

    public constructor(init?: Partial<CatalogueResourceModel>) {
        Object.assign(this, init);
    }
}

export class CatalogueResourceResultModel {
    nodeId: number;
    totalResources: number;
    catalogueResources: CatalogueResourceModel[];
    constructor(init?: Partial<CatalogueResourceResultModel>) {
        Object.assign(this, init);
    }
}

export enum ResourceOrderEnum {    
    AlphabeticalAscending = 0,
    DateDescending = 1
}
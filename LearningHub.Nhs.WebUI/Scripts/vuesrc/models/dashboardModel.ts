import { ResourceType } from "../constants";

export class CatalogueCardModel {
    nodeId: number;
    name: string;    
    description: string;
    url: string;
    badgeUrl: string;
    bannerUrl: string;
    restrictedAccess: boolean;
    hasAccess: boolean;
    bookmarkId: number;
    isBookmarked: boolean;    
    public constructor(init?: Partial<CatalogueCardModel>) {
        Object.assign(this, init);
    }
}

export class CatalogueCardsResultModel {
    type: string;
    catalogues: CatalogueCardModel[];    
    totalCount: number;
    constructor(init?: Partial<CatalogueCardsResultModel>) {
        Object.assign(this, init);
    }
}

import { PageSectionDetailModel } from "./pageSectionDetailModel";

export class PageSectionModel{ 
    id: number; 
    pageId: number; 
    position: number; 
    isHidden: boolean;     
    sectionTemplateType: SectionTemplateType; 
    amendUserId: number; 
    pageSectionDetail: PageSectionDetailModel;

    public constructor(init?: Partial<PageSectionModel>) {
        Object.assign(this, init);
    }
}

export enum PageSectionStatus {
    Draft = 1,
    Processing = 2,
    ProcessingFailed = 3,
    Processed = 4,
    Live = 5
}

export enum SectionTemplateType {
    Image = 1,
    Video = 2
}


export class UpdatePageSectionOrderModel {
    pageSectionId: number;
    pageId: number;
    directionType: DirectionType;
    
    public constructor(init?: Partial<UpdatePageSectionOrderModel>) {
        Object.assign(this, init);
    }
}

export enum DirectionType {
    Up = 1,
    Down = 2
}
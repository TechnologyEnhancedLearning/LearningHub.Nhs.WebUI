import { ImageAssetModel } from "./imageAssetModel";
import { PageSectionModel, PageSectionStatus } from "./pageSectionModel";
import { VideoAssetModel } from "./videoAssetModel";

export class PageSectionDetailModel {
    id: number;
    pageSectionId: number;
    sectionTitle: string;
    sectionTitleElement: string = "h2";
    topMargin: boolean;
    bottomMargin: boolean;
    hasBorder: boolean;
    backgroundColour: string ="#FFFFFF";
    textColour: string ="#212B32";
    hyperLinkColour: string = "#005EB8";
    textBackgroundColour: string = "#FFFFFF";
    description: string;
    sectionLayoutType: SectionLayoutType = SectionLayoutType.Left;
    pageSectionStatus: PageSectionStatus = PageSectionStatus.Draft;
    amendUserId: number;
    pageSection: PageSectionModel;
    imageAsset: ImageAssetModel;
    videoAsset: VideoAssetModel;
    deletePending: boolean;
    draftPosition: number | null;
    draftHidden: boolean | null;

    public constructor(init?: Partial<PageSectionDetailModel>) {
        Object.assign(this, init);
    }
}

export enum SectionLayoutType {
    Left = 1,
    Right = 2
}

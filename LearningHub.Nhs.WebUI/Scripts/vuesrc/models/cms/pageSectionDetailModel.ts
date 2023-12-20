import { ImageAssetModel } from "./imageAssetModel";
import { PageSectionModel, PageSectionStatus } from "./pageSectionModel";
import { VideoAssetModel } from "./videoAssetModel";

export class PageSectionDetailModel {
    id: number;
    pageSectionId: number;
    title: string;
    backgroundColour: string = "#FFFFFF";
    textColour: string = "#212B32";
    hyperLinkColour: string = "#005EB8";
    description: string;
    sectionLayoutType: SectionLayoutType = SectionLayoutType.Left;
    assetPosition: AssetPosition = AssetPosition.Center;
    pageSectionStatus: PageSectionStatus = PageSectionStatus.Draft;
    amendUserId: number;
    pageSection: PageSectionModel;
    imageAsset: ImageAssetModel;
    videoAsset: VideoAssetModel;
    draftHidden: boolean | null;
    draftPosition: number | null;
    deletePending: boolean | null;

    public constructor(init?: Partial<PageSectionDetailModel>) {
        Object.assign(this, init);
    }
}

export enum AssetPosition {
    Top = 1,
    Center = 2,
    Bottom = 3
}

export enum SectionLayoutType {
    Left = 1,
    Right = 2
}
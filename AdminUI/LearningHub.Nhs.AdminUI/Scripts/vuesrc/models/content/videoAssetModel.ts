import { FileModel } from "./fileModel";
import { PageSectionDetailModel } from "./pageSectionDetailModel";

export class VideoAssetModel{ 
    id: number; 
    pageSectionDetailId: number;     
    amendUserId: number;
    videoFileId: number;
    azureMediaAssetId: number;
    transcriptFileId: number;
    closedCaptionsFileId: number;
    thumbnailImageFileId: number;
    azureMediaAsset: AzureMediaAssetModel;
    closedCaptionsFile: FileModel;
    pageSectionDetail: PageSectionDetailModel; 
    thumbnailImageFile: FileModel;
    transcriptFile: FileModel;
    videoFile: FileModel;

    public constructor(init?: Partial<VideoAssetModel>) {
        Object.assign(this, init);
    }
}

export class AzureMediaAssetModel {
    azureMediaAssetId: number;
    filePath: string;
    locatorUri: string;
    authenticationToken: string;

    public constructor(init?: Partial<AzureMediaAssetModel>) {
        Object.assign(this, init);
    }
}

export class VideoResourceModel {
    resourceVersionId: number = 0;
    file: ResourceFileModel = new ResourceFileModel();
    transcriptFile: ResourceFileModel = null;
    closedCaptionsFile: ResourceFileModel = null;
}

export class ResourceFileModel {
    resourceVersionId: number = 0;
    fileId: number = 0;
    fileName: string = '';
    fileTypeId: number = 0;
    fileSizeKb: number = 0;
    fileLocation: string = '';
}
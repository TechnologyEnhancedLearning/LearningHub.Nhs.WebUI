import { FileModel } from './fileModel';
import { ResourceAzureMediaAssetModel } from './resourceAzureMediaAssetModel';

export class VideoModel {
    resourceVersionId: number;
    fileId: number;    
    file: FileModel;
    transcriptFileId: number;
    transcriptFile: FileModel;
    closedCaptionFileId: number;
    closedCaptionFile: FileModel;
    duration: number;  
    durationInMilliseconds: number;
    resourceAzureMediaAsset: ResourceAzureMediaAssetModel;

    public constructor(init?: Partial<VideoModel>) {
        Object.assign(this, init);
    }
}
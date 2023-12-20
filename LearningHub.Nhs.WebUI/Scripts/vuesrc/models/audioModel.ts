import { FileModel } from './fileModel';
import { ResourceAzureMediaAssetModel } from './resourceAzureMediaAssetModel';

export class AudioModel {
    resourceVersionId: number;
    fileId: number;    
    file: FileModel;
    transcriptFileId: number;
    transcriptFile: FileModel;        
    durationInMilliseconds: number;
    resourceAzureMediaAsset: ResourceAzureMediaAssetModel;

    public constructor(init?: Partial<AudioModel>) {
        Object.assign(this, init);
    }
}
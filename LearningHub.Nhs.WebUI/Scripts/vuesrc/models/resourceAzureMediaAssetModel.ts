export class ResourceAzureMediaAssetModel {
    resourceAzureMediaAssetId: number;    
    filePath: string;    
    locatorUri: string;    
    authenticationToken: string;

    public constructor(init?: Partial<ResourceAzureMediaAssetModel>) {
        Object.assign(this, init);
    }
}
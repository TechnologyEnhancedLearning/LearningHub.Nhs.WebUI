import { FileModel } from "./fileModel";
import { PageSectionDetailModel } from "./pageSectionDetailModel";

export class ImageAssetModel{ 
    id: number; 
    pageSectionDetailId: number; 
    imageFileId: number; 
    altTag: string; 
    description: string; 
    imageFile: FileModel;
    pageSectionDetail: PageSectionDetailModel;

    public constructor(init?: Partial<ImageAssetModel>) {
        Object.assign(this, init);
    }
}

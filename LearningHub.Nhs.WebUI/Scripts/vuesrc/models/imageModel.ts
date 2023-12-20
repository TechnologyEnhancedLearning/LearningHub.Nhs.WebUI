import { FileModel } from './fileModel';

export class ImageModel {
    resourceVersionId: number;
    imageFileId: number;
    file: FileModel;
    altTag: string;    
    description: string;

    public constructor(init?: Partial<ImageModel>) {
        Object.assign(this, init);
    }
}
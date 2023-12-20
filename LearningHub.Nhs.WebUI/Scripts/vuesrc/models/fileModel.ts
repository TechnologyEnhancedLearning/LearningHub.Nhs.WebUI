import { FileTypeModel } from "./contribute/fileTypeModel";

export class FileModel {
    fileId: number;
    fileName: string;
    fileSizeKb: number;
    fileTypeId: number;
    filePath: string;    
    fileType: FileTypeModel;

    public constructor(init?: Partial<FileModel>) {
        Object.assign(this, init);
    }
}
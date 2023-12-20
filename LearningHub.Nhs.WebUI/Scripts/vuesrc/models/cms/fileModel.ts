import { FileTypeModel } from "./fileTypeModel";

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


export class PageSectionDetailFileModel extends FileModel{
    pageSectionDetailId: number;
    fileLocation: string;

    public constructor(init?: Partial<PageSectionDetailFileModel>) {
        super();
        Object.assign(this, init);
    }
}
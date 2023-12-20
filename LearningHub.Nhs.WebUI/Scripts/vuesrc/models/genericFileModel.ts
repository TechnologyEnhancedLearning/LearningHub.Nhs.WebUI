import { FileModel } from './fileModel';

export class GenericFileModel {
    resourceVersionId: number;
    fileId: number;    
    file: FileModel;
    scormAiccContent: boolean = false;
    authoredYear: number;
    authoredMonth: number;
    authoredDayOfMonth: number;
    nextReviewDate: string;

    public constructor(init?: Partial<GenericFileModel>) {
        Object.assign(this, init);
    }
}
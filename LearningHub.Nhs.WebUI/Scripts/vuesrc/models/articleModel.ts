import { FileModel } from './fileModel';

export class ArticleModel {
    resourceVersionId: number;
    dateCreated: string;
    nextReviewDate: string;    
    description: string;
    files: FileModel[];

    public constructor(init?: Partial<ArticleModel>) {
        Object.assign(this, init);
    }
}
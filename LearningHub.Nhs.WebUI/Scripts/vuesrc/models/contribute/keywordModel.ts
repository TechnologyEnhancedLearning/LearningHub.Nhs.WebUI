export class KeywordModel {
    id: number;
    keyword: string;
    resourceVersionId: number;

    public constructor(init?: Partial<KeywordModel>) {
        Object.assign(this, init);
    }
}
export class EmbedCodeModel {
    resourceVersionId: number;
    embedCode: string;    

    constructor(init?: Partial<EmbedCodeModel>) {
        Object.assign(this, init);
    }
}

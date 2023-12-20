export class WebLinkModel {
    resourceVersionId: number;
    displayText: string;    
    url: string;

    public constructor(init?: Partial<WebLinkModel>) {
        Object.assign(this, init);
    }
}
export class CatalogueLocationModel {
    catalogueId: number;
    catalogueName: string;
    icon: string;
    resourceCount: number;
    public constructor(init?: Partial<CatalogueLocationModel>) {
        Object.assign(this, init);
    }
}
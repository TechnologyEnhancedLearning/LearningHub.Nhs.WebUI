export class CatalogueBasicModel {
    catalogueNodeVersionId: number;
    nodeId: number;
    name: string;
    rootNodePathId: number;
    hidden: boolean;
    url: string;
    restrictedAccess: boolean;
    public constructor(init?: Partial<CatalogueBasicModel>) {
        Object.assign(this, init);
    }
}
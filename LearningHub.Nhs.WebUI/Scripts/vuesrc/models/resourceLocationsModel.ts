import { CatalogueLocationModel } from './catalogueLocationModel';
export class ResourceLocationsModel  {

    id: number;

    locations: CatalogueLocationModel[];

    constructor(init?: Partial<ResourceLocationsModel>) {
        Object.assign(this, init);
    }
}
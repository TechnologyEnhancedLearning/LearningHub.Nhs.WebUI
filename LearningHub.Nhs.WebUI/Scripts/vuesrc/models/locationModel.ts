export class LocationModel {
    id: number;
    name: string;
    address: string;
    nhsCode: string;
    expandedName: string;
    active: boolean;

    constructor(init?: Partial<LocationModel>) {
        Object.assign(this, init);
    }
}

export class AddressModel {
    id: number;
    address1: string;
    address2: string;
    address3: string;
    address4: string;
    town: string;
    county: string;
    postCode: string;

    constructor(init?: Partial<AddressModel>) {
        Object.assign(this, init);
    }
}

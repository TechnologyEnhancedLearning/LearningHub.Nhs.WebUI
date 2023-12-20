import { AddressModel } from './addressModel';

export class EquipmentModel {
    resourceVersionId: number;
    contactName: string;
    contactTelephone: string;
    contactEmail: string;
    address: AddressModel;

    constructor(init?: Partial<EquipmentModel>) {
        Object.assign(this, init);
    }
}

export class LicenceModel {
    id: number;
    title: string;

    public constructor(init?: Partial<LicenceModel>) {
        Object.assign(this, init);
    }
}
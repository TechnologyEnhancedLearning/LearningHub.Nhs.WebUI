export class ProviderModel {
    id: number;
    name: string;   

    public constructor(init?: Partial<ProviderModel>) {
        Object.assign(this, init);
    }
}
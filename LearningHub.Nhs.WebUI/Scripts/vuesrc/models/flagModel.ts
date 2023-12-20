export class FlagModel {
    id: number;
    resourceVersionId: number;
    flaggedByUserId: number;
    flaggedByName: string;
    details: string;
    isActive: boolean;
    resolution: string;

    constructor(init?: Partial<FlagModel>) {
        Object.assign(this, init);
    }
}

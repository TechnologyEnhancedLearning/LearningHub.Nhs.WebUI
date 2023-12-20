export class InteractionQueueModel {
    mediaResourceActivityType: string;
    currentMediaTime: number;
    clientDateTime: Date;

    public constructor(init?: Partial<InteractionQueueModel>) {
        Object.assign(this, init);
    }
}
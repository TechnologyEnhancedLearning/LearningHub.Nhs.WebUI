export class LearningHubValidationResultModel {
    isValid: boolean;
    details: string[];
    createdId: number;

    constructor(init?: Partial<LearningHubValidationResultModel>) {
        Object.assign(this, init);
    }
}
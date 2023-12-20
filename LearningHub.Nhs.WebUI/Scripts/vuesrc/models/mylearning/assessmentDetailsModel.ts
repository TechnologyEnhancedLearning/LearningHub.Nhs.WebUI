export class AssessmentDetailsModel {
    passMark?: number;
    maximumAttempts?: number;
    currentAttempt?: number;
    extraAttemptReason: string;
    
    public constructor(init?: Partial<AssessmentDetailsModel>) {
        Object.assign(this, init);
    }
}

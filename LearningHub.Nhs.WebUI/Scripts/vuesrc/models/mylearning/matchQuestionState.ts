export class MatchQuestionState {
    questionNumber: number;
    firstMatchAnswerId: number;
    secondMatchAnswerId: number;
    order: number;
    
    public constructor(init?: Partial<MatchQuestionState>) {
        Object.assign(this, init);
    }
}

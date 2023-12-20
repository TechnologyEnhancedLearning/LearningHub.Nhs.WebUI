import { AssessmentModel } from '../contribute-resource/assessmentModel';
import { MatchQuestionState } from "./matchQuestionState";

export class AssessmentProgressModel {
    maxScore: number;
    userScore: number;
    assessmentViewModel: AssessmentModel;
    numberOfAttempts: number;
    assessmentResourceActivityId: number;
    answers: number[][];
    passed: boolean;
    matchQuestions: MatchQuestionState[];
    
    constructor(init?: Partial<AssessmentProgressModel>) {
        if (init) {
            this.maxScore = init.maxScore;
            this.userScore = init.userScore;
            this.assessmentViewModel = new AssessmentModel(init.assessmentViewModel);
            this.numberOfAttempts = init.numberOfAttempts;
            this.answers = init.answers;
            this.assessmentResourceActivityId = init.assessmentResourceActivityId;
            this.passed = init.passed;
            this.matchQuestions = init.matchQuestions;
        }
    }
}

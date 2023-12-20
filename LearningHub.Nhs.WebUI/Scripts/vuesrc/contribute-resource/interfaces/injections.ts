import { ResourceType } from "../../constants";
import { AssessmentModel } from "../../models/contribute-resource/assessmentModel";

export interface ContributeCaseQuestionsInjection {
    questionData: {
        currentQuestion: number;
    };
    updateCurrentQuestion: (val: number) => void;
}

export interface ContributeInjection {
    resourceType: ResourceType;
    assessmentDetails?: AssessmentModel;
}
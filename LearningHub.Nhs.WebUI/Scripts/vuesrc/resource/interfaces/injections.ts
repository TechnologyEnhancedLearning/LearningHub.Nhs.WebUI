import { ResourceType } from "../../constants";
import { AssessmentModel } from "../../models/contribute-resource/assessmentModel";

export interface ResourceInjection {
    resourceType: ResourceType;
    assessmentDetails?: AssessmentModel;
}
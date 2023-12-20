import { ResourceType } from "../../constants";
import { AssessmentModel } from "../../models/contribute-resource/assessmentModel";
import { AssessmentTypeEnum } from "../../models/contribute-resource/blocks/assessments/assessmentTypeEnum";

export function resourceTypeHasAllowReveal(resourceType: ResourceType) {
    return resourceType === ResourceType.CASE;
}

export function resourceTypeHasFeedback(resourceType: ResourceType, assessmentDetails?: AssessmentModel) {
    return resourceType === ResourceType.CASE || (resourceType === ResourceType.ASSESSMENT && assessmentDetails.assessmentType === AssessmentTypeEnum.Informal);
}
import { ResourceType } from '../../../constants';
import { AssessmentModel } from '../../../models/contribute-resource/assessmentModel';
import { AssessmentTypeEnum } from '../../../models/contribute-resource/blocks/assessments/assessmentTypeEnum';
import { resourceTypeHasAllowReveal, resourceTypeHasFeedback } from '../CaseOrAssessmentsHelper';

describe('CaseOrAssessmentsHelper', () => {
    describe('resourceTypeHasAllowReveal', () => {
        test('True for cases, false for others', () => {
            expect(resourceTypeHasAllowReveal(ResourceType.CASE)).toBe(true);
            expect(resourceTypeHasAllowReveal(ResourceType.ASSESSMENT)).toBe(false);
            expect(resourceTypeHasAllowReveal(ResourceType.UNDEFINED)).toBe(false);
        });
    });

    describe('resourceTypeHasFeedback', () => {
        test('True for cases', () => {
            expect(resourceTypeHasFeedback(ResourceType.CASE)).toBe(true);
        });

        test('True for informal assessments', () => {
            const assessment = new AssessmentModel();
            assessment.assessmentType = AssessmentTypeEnum.Informal;
            expect(resourceTypeHasFeedback(ResourceType.ASSESSMENT, assessment)).toBe(true);
        });

        test('False for formal assessments', () => {
            const assessment = new AssessmentModel();
            assessment.assessmentType = AssessmentTypeEnum.Formal;
            expect(resourceTypeHasFeedback(ResourceType.ASSESSMENT, assessment)).toBe(false);
        });
    });
});

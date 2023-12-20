import { AssessmentTypeEnum } from './blocks/assessments/assessmentTypeEnum';
import { BlockCollectionModel } from './blocks/blockCollectionModel';
import { BlockTypeEnum } from './blocks/blockTypeEnum';

export class AssessmentModel {
    resourceVersionId: number = 0;
    assessmentType: AssessmentTypeEnum;
    assessmentContent: BlockCollectionModel;
    maximumAttempts?: number;
    passMark?: number;
    answerInOrder: boolean;
    endGuidance: BlockCollectionModel;
    assessmentSettingsAreValid: boolean;    //Is endGuidance valid, and if a formal assessment is there a pass mark.
    
    constructor(init?: Partial<AssessmentModel>) {
        if (init) {
            this.resourceVersionId = init.resourceVersionId;
            this.assessmentType = init.assessmentType;
            this.assessmentContent = new BlockCollectionModel(init.assessmentContent);
            this.endGuidance = new BlockCollectionModel(init.endGuidance);
            this.passMark = init.passMark;
            this.answerInOrder = init.answerInOrder;
            this.maximumAttempts = init.maximumAttempts;
            this.assessmentSettingsAreValid = true; // Set to true initially as it is not possible to have saved an invalid state.
        }
    }

    isReadyToPublish(): boolean {
        const settingsReadyToPublish = this.assessmentType !== AssessmentTypeEnum.Formal || this.passMark;

        return settingsReadyToPublish && this.assessmentContent.isReadyToPublish() 
            && (this.endGuidance.isReadyToPublish() && this.assessmentSettingsAreValid)
            && this.assessmentContent.blocks.some(b => b.blockType === BlockTypeEnum.Question);
    }
}

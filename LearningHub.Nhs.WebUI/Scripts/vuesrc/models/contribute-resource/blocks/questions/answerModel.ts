import { BlockCollectionModel } from "../blockCollectionModel";
import { AnswerTypeEnum } from "./answerTypeEnum";
import { BlockTypeEnum } from "../blockTypeEnum";

export class AnswerModel {
    status: AnswerTypeEnum;
    order: number;
    blockCollection: BlockCollectionModel;
    id: number;
    imageAnnotationOrder: number;

    constructor(init?: Partial<AnswerModel>, imageZone?: boolean) {
        if (init) {
            this.status = init.status;
            if (!imageZone) {
                this.blockCollection = new BlockCollectionModel(init.blockCollection);
            }
            this.order = init.order;
            this.id = init.id;
            this.imageAnnotationOrder = init.imageAnnotationOrder
        }
    }

    isReadyToPublish(isMatchGame: boolean = false): boolean {
        let isReadyToPublish = typeof this.status === 'number'
            && typeof this.imageAnnotationOrder === 'number'
            || (!!this.blockCollection &&
                this.blockCollection.blocks
                    .filter(block => block.blockType === BlockTypeEnum.Text)
                    .every(block => block.textBlock.content.length <= 120) &&
                this.blockCollection.isReadyToPublish());

        return isReadyToPublish && (!isMatchGame || this.blockCollection.blocks.length === 2);
    }
    
    getBlockType(index: number) {
        return this.blockCollection?.blocks?.[index].blockType;
    }
}

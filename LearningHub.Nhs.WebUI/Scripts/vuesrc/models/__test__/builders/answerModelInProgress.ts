import {AnswerTypeEnum} from "../../contribute-resource/blocks/questions/answerTypeEnum";
import {BlockCollectionModel} from "../../contribute-resource/blocks/blockCollectionModel";
import {BlockTypeEnum} from "../../contribute-resource/blocks/blockTypeEnum";
import {AnswerModel} from "../../contribute-resource/blocks/questions/answerModel";

export function createAnswerModel() {
    return new AnswerModelInProgress();
}

class AnswerModelInProgress {
    status: AnswerTypeEnum = AnswerTypeEnum.Incorrect;
    order: number = 0;
    blockCollection: BlockCollectionModel = new BlockCollectionModel();
    
    constructor() {
        this.blockCollection.addBlock(BlockTypeEnum.Text);
    }
    
    withTextContent(text: string) {
        this.blockCollection.blocks[0].textBlock.content = text;
        return this;
    }
    
    withStatus(status: AnswerTypeEnum) {
        this.status = status;
        return this;
    }
    
    build() {
        const answerModel = new AnswerModel();
        answerModel.status = this.status;
        answerModel.order = this.order;
        answerModel.blockCollection = this.blockCollection;
        return answerModel;
    }
}
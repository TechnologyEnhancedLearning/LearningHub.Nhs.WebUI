import {QuestionTypeEnum} from "../../contribute-resource/blocks/questions/questionTypeEnum";
import {BlockCollectionModel} from "../../contribute-resource/blocks/blockCollectionModel";
import {AnswerModel} from "../../contribute-resource/blocks/questions/answerModel";
import {BlockTypeEnum} from "../../contribute-resource/blocks/blockTypeEnum";
import {QuestionBlockModel} from "../../contribute-resource/blocks/questionBlockModel";
import {AnswerTypeEnum} from "../../contribute-resource/blocks/questions/answerTypeEnum";
import {createAnswerModel} from "../builders/answerModelInProgress";

export const newQuestionBlock = () => {
    return new QuestionBlockInProgress();
}

class QuestionBlockInProgress {
    questionType: QuestionTypeEnum = undefined;
    questionBlockCollection: BlockCollectionModel = undefined;
    answers: AnswerModel[] = undefined;
    feedbackBlockCollection: BlockCollectionModel = undefined;
    allowReveal: boolean = undefined;
    
    constructor() {
        this.questionType = QuestionTypeEnum.SingleChoice;
        
        this.questionBlockCollection = new BlockCollectionModel();
        this.questionBlockCollection.addBlock(BlockTypeEnum.Text);
        this.questionBlockCollection.blocks[0].textBlock.content = "This is the content";
        
        this.answers = [
        createAnswerModel().withTextContent("This is an answer").withStatus(AnswerTypeEnum.Best).build(),
        createAnswerModel().withTextContent("This is an answer").build()];
        
        this.feedbackBlockCollection = new BlockCollectionModel();
        this.feedbackBlockCollection.addBlock(BlockTypeEnum.Text);
        this.feedbackBlockCollection.blocks[0].textBlock.content = "This is the content";
        
        this.allowReveal = true;
    }
    
    withQuestionType = (questionType: QuestionTypeEnum) => {
        this.questionType = questionType;
        return this;
    }
    
    withQuestionContent = (content: string) => {
        this.questionBlockCollection.blocks[0].textBlock.content = content;
        return this;
    }

    withFeedback = (feedback: string) => {
        this.feedbackBlockCollection.blocks[0].textBlock.content = feedback;
        return this;
    }
    
    withAllowReveal = (allowReveal: boolean) => {
        this.allowReveal = allowReveal;
        return this;
    }
    
    withAnswersOfStatus = (answerTypes: AnswerTypeEnum[]) => {
        this.answers = answerTypes.map(answerType => createAnswerModel().withTextContent("This is an answer").withStatus(answerType).build());
        return this;
    }
    
    withAnswer = (answer: AnswerModel, index: number) => {
        this.answers[index] = new AnswerModel(answer);
        return this;
    }
    
    build = () => {
        return new QuestionBlockModel({
            questionType: this.questionType,
            questionBlockCollection: this.questionBlockCollection,
            answers: this.answers,
            feedbackBlockCollection: this.feedbackBlockCollection,
            allowReveal: this.allowReveal
        })
    }
}
import { BlockCollectionModel } from "./blockCollectionModel";
import { BlockTypeEnum } from "./blockTypeEnum";
import { AnswerModel } from "./questions/answerModel";
import { AnswerTypeEnum } from "./questions/answerTypeEnum";
import { QuestionTypeEnum } from "./questions/questionTypeEnum";
import { MatchGameSettings } from "./questions/matchGameSettings";
import { MediaTypeEnum } from "./mediaTypeEnum";

export class QuestionBlockModel {
    questionType: QuestionTypeEnum;
    questionBlockCollection: BlockCollectionModel;
    answers: AnswerModel[];
    feedbackBlockCollection: BlockCollectionModel;
    allowReveal: boolean;
    matchGameSettings: MatchGameSettings;

    constructor(init?: Partial<QuestionBlockModel>) {
        if (init) {
            this.questionType = init.questionType;
            this.questionBlockCollection = new BlockCollectionModel(init.questionBlockCollection);
            this.answers = init.answers.map(answer => new AnswerModel(answer, this.questionType === QuestionTypeEnum.ImageZone))
                .sort((a, b) => a.order - b.order);
            this.feedbackBlockCollection = new BlockCollectionModel(init.feedbackBlockCollection);
            this.allowReveal = init.allowReveal;
            this.setMatchGameSettings();
        }
        else {
            this.questionBlockCollection = new BlockCollectionModel();
            this.answers = [];
            this.feedbackBlockCollection = new BlockCollectionModel();
        }
    }

    isReadyToPublish(): boolean {
        return this.questionBlockCollection &&
            this.answers &&
            (typeof this.allowReveal !== 'undefined') &&
            this.questionBlockCollection.isReadyToPublish() &&
            this.answerIsReadyToPublish() &&
            this.feedbackBlockCollection.isReadyToPublish(); // Returns true if all question boxes filled in
    }

    setMatchGameSettings() {
        const isMatchGameWithAtLeastOneAnswer = this.isMatchGame() && this.answers.length;
        if (isMatchGameWithAtLeastOneAnswer) {
            const firstAnswer = this.answers[0];
            const firstColumnBlockType = firstAnswer.getBlockType(0); 
            const secondColumnBlockType = firstAnswer.getBlockType(1);
            this.matchGameSettings = this.getMatchGameSettings(firstColumnBlockType, secondColumnBlockType);
        }
    }

    getMatchGameSettings(firstColumnBlockType: BlockTypeEnum, secondColumnBlockType: BlockTypeEnum) {
        if (firstColumnBlockType !== secondColumnBlockType) {
            return MatchGameSettings.ImageToText;
        }
        if (firstColumnBlockType === BlockTypeEnum.Text) {
            return MatchGameSettings.TextToText;
        }
        return MatchGameSettings.ImageToImage;
    }
    
    isTitleReady(): boolean {
        if (this.questionType === QuestionTypeEnum.ImageZone) {
            return this.questionBlockCollection.blocks[0]?.isReadyToPublish();
        } else {
            return this.questionBlockCollection.isReadyToPublish();
        }
    }

    addAnswer(answerType?: AnswerTypeEnum, order?: number) {
        const newAnswer = new AnswerModel();
        if (answerType !== undefined) {
            newAnswer.status = answerType;
        }
        if (this.questionType !== QuestionTypeEnum.ImageZone) {
            newAnswer.blockCollection = new BlockCollectionModel();
            if (this.isMatchGame()) {
                switch (this.matchGameSettings) {
                    case MatchGameSettings.ImageToText:
                        newAnswer.blockCollection.addMediaBlock(undefined, MediaTypeEnum.Image);
                        newAnswer.blockCollection.addBlock(BlockTypeEnum.Text);
                        break;
                    case MatchGameSettings.ImageToImage:
                        newAnswer.blockCollection.addMediaBlock(undefined, MediaTypeEnum.Image);
                        newAnswer.blockCollection.addMediaBlock(undefined, MediaTypeEnum.Image);
                        break;
                    default:
                        newAnswer.blockCollection.addBlock(BlockTypeEnum.Text);
                        newAnswer.blockCollection.addBlock(BlockTypeEnum.Text);
                        break;
                }
        
            } else {
                newAnswer.blockCollection.addBlock(BlockTypeEnum.Text);
            }
        }
        
        newAnswer.order = this.answers.length;
        if (order !== undefined) {
            newAnswer.imageAnnotationOrder = order;
        }
        this.answers.push(newAnswer);
    }

    answerBelongsToThisQuestion(answer: AnswerModel): boolean {
        return (this.answers.indexOf(answer) !== -1);
    }

    moveAnswerUp(answer: AnswerModel): void {
        if (this.answerBelongsToThisQuestion(answer)) {
            answer.order -= 1.5;
            answer.imageAnnotationOrder -= 1.5;
            this.sortAndReNumberAnswers();
        }
    }

    moveAnswerDown(answer: AnswerModel): void {
        if (this.answerBelongsToThisQuestion(answer)) {
            answer.order += 1.5;
            answer.imageAnnotationOrder += 1.5;
            this.sortAndReNumberAnswers();
        }
    }

    sortAndReNumberAnswers(): void {
        this.sortAnswers();
        this.answers.forEach((aEl, aIndex) => aEl.order = aIndex);
        this.answers.forEach((aEl, aIndex) => aEl.imageAnnotationOrder = aIndex);
    }

    sortAnswers(): void {
        this.answers.sort((a1, a2) => a1.order - a2.order);
    }

    isFull() {
        return this.answers.length >= this.getMaximumNumberOfAnswers(); 
    }

    getMaximumReachedMessage() : string {
        return `You have added the maximum of ${ this.getMaximumNumberOfAnswers() } annotations, you can edit these if you want to make changes`;
    }

    getMaximumNumberOfAnswers() {
        return this.isMatchGame() || this.questionType === QuestionTypeEnum.ImageZone ? 10 : 20;
    }

    oneBestAnswer() {
        return this.answers.filter(answer => answer.status === AnswerTypeEnum.Best).length === 1;
    }

    twoAnswers() {
        return (this.answers.length >= 2);
    }

    isSingleChoiceWithOneBestAnswer() {
        return this.questionType === QuestionTypeEnum.SingleChoice && this.oneBestAnswer();
    }

    isMultipleChoiceWithNoBestAnswers() {
        return this.questionType === QuestionTypeEnum.MultipleChoice && this.answers.every(answer => answer.status !== AnswerTypeEnum.Best);
    }
    
    isMatchGame() {
        return this.questionType === QuestionTypeEnum.MatchGame;
    }

    isImageZoneWithAtLeastOneBestAnswer() {
        return this.questionType === QuestionTypeEnum.ImageZone
            && this.questionBlockCollection.blocks.length === 2
            && this.questionBlockCollection.blocks[1].isReadyToPublish()
            && this.answers.filter(answer => answer.status === AnswerTypeEnum.Best).length >= 1;
    }
    
    answerIsReadyToPublish() {
        return this.answers.every(answer => answer.isReadyToPublish(this.isMatchGame())) && this.twoAnswers() &&
            (this.isSingleChoiceWithOneBestAnswer() || this.isMultipleChoiceWithNoBestAnswers() || this.isImageZoneWithAtLeastOneBestAnswer() || this.isMatchGame()) &&
            this.answers.length <= this.getMaximumNumberOfAnswers();
    }
    
    deleteAnswer(answer: AnswerModel) {
        const index = this.answers.indexOf(answer);
        this.answers.splice(index, 1);
        this.setAnswerOrder();
    }

    setAnswerOrder() {
        this.answers.forEach((answer, answerIndex) => answer.order = answerIndex);
        if (this.questionType == QuestionTypeEnum.ImageZone) {
            this.answers.forEach((answer, answerIndex) => answer.imageAnnotationOrder = answerIndex);
        }
    }
}
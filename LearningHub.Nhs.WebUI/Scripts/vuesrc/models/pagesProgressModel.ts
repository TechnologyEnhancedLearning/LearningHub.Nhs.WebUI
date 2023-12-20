import { PageStatusEnum } from "./pageStatusEnum";
import { BlockCollectionModel } from "./contribute-resource/blocks/blockCollectionModel";
import { BlockModel } from "./contribute-resource/blocks/blockModel";
import { BlockTypeEnum } from "./contribute-resource/blocks/blockTypeEnum";

export class PagesProgressModel {
    blockCollection: BlockCollectionModel;
    pageStatuses: PageStatusEnum[];
    questionProgress: number[][];

    public constructor(blockCollection: BlockCollectionModel) {
        this.blockCollection = blockCollection;
        this.pageStatuses = this.getInitialPageStates();
        this.questionProgress = blockCollection.blocks
            .filter((block: BlockModel) => block.blockType === BlockTypeEnum.Question)
            .map(_ => undefined);
    }

    public readingPage(pageNumber: number) {
        this.pageStatuses = this.pageStatuses.map((status, index) => {
            if (this.isPreviouslyReadPage(index, pageNumber)) {
                return PageStatusEnum.Completed;
            }

            if (index === pageNumber && this.isIncomplete(index)) {
                return PageStatusEnum.Reading;
            }

            if (this.isReadingPage(index)) {
                return PageStatusEnum.Available;
            }

            if (this.isNonQuestionPage(pageNumber) && index === pageNumber + 1 && this.isIncomplete(index)) {
                return PageStatusEnum.Available;
            }

            return status;
        });
    }

    public readingPageAnswerInAnyOrder(pageNumber: number) {
        this.pageStatuses = this.pageStatuses.map((status, index) => {
            if (index === pageNumber && this.isIncomplete(index)) {
                return PageStatusEnum.Reading;
            }
            if (this.isIncomplete(index)) {
                return PageStatusEnum.Available;
            }
            return status;
        });
    }

    public get allPagesCompleted() {
        return this.pageStatuses.every(status => status === PageStatusEnum.Completed);
    }

    public isNonQuestionPage(pageNumber: number) {
        const pages = this.blockCollection.getPages();
        return !this.pageContainsQuestion(pages[pageNumber]);
    }

    public isIncomplete(pageNumber: number) {
        return this.pageStatuses[pageNumber] !== PageStatusEnum.Completed;
    }

    public completePage(completedPage: number) {
        this.pageStatuses = this.pageStatuses.map((current, index) => {
            if (index === completedPage) {
                return PageStatusEnum.Completed;
            } else if (index === completedPage + 1 && this.pageStatuses[index] !== PageStatusEnum.Completed) {
                return PageStatusEnum.Available;
            }
            return current;
        });
    }

    public updateQuestionProgress(question: number, answers: number[]) {
        this.questionProgress[question] = answers;
    }

    public getQuestionProgressByPage(page: number) {
        this.blockCollection.sortAndReNumberBlocks();
        const blocksOnPage = this.blockCollection.getBlocksByPage(page);
        const questionBlocksOnPage = blocksOnPage.filter(block => block.blockType === BlockTypeEnum.Question);
        const questionBlocks = this.blockCollection.blocks.filter(block => block.blockType === BlockTypeEnum.Question);
        const questionIndexesOnPage = questionBlocksOnPage.map(questionBlock => questionBlocks.indexOf(questionBlock));
        return questionIndexesOnPage.map((questionIndex, index) => this.questionProgress[questionIndex]);
    }

    public initialiseQuestionProgress(answers: number[][]) {
        this.questionProgress = answers.map(selection => this.isUnansweredQuestion(selection) ? undefined : selection);

        const pages = this.blockCollection.getPages();
        pages.forEach((page, index) => {
            const pageProgress = this.getQuestionProgressByPage(index);
            if (pageProgress.every(questionAnswer => questionAnswer !== undefined) && pageProgress.length > 0) {
                this.completePage(index);
            } else if (pageProgress.some(questionAnswer => questionAnswer !== undefined) && pageProgress.length > 0) {
                this.readingPage(index);
            }
        });
    }

    public getAvailableIncompletePage() {
        return Math.max(0, (this.pageStatuses.findIndex(page => page === PageStatusEnum.Available || page === PageStatusEnum.Reading)));
    }

    private isReadingPage(pageNumber: number) {
        return this.pageStatuses[pageNumber] === PageStatusEnum.Reading;
    }

    private isPreviouslyReadPage(pageNumber: number, referencePage: number) {
        const isPrevious = pageNumber < referencePage;
        const isNonQuestionReadingPage = this.isNonQuestionPage(pageNumber) && this.isReadingPage(pageNumber);

        return isPrevious || isNonQuestionReadingPage;
    }

    private getInitialPageStates() {
        // If the first page contains no question, [Reading, Available, ...Locked...]
        // If the first page contains a question, [Reading, ...Locked...]
        const pages = this.blockCollection.getPages();

        const firstPageContainsQuestions = this.pageContainsQuestion(pages[0]);

        return pages.map((_, index) => {
            if (index === 0) {
                return PageStatusEnum.Reading;
            } else if (index === 1 && !firstPageContainsQuestions) {
                return PageStatusEnum.Available;
            } else {
                return PageStatusEnum.Locked;
            }
        });
    }

    private pageContainsQuestion(page: BlockModel[]) {
        return !!page && page.some(block => block.blockType === BlockTypeEnum.Question);
    }

    private isUnansweredQuestion(selection: number[]) {
        return selection.length === 1 && selection[0] === -1;
    }


}

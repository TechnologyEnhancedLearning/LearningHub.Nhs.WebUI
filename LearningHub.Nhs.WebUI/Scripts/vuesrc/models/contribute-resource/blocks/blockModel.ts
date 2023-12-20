import {TextBlockModel} from './textBlockModel';
import {WholeSlideImageBlockModel} from './wholeSlideImageBlockModel';
import {BlockTypeEnum} from './blockTypeEnum';
import {MediaBlockModel} from "./mediaBlockModel";
import {QuestionBlockModel} from "./questionBlockModel";
import {ImageCarouselBlockModel} from "./imageCarouselBlockModel";

let nextBlockRef = 0;

export class BlockModel {
    id: number = undefined;
    blockType: BlockTypeEnum = undefined;
    order: number = undefined;
    title: string = undefined;
    textBlock: TextBlockModel = undefined;
    wholeSlideImageBlock: WholeSlideImageBlockModel = undefined;
    mediaBlock: MediaBlockModel = undefined;
    questionBlock: QuestionBlockModel = undefined;
    imageCarouselBlock: ImageCarouselBlockModel = undefined;

    blockRef: number = null;

    constructor(init?: Partial<BlockModel>) {
        this.blockRef = nextBlockRef++;

        if (init) {
            this.id = init.id;
            this.blockType = init.blockType;
            this.order = init.order;
            this.title = init.title;

            if (init.textBlock) { this.textBlock = new TextBlockModel(init.textBlock); }
            if (init.wholeSlideImageBlock) { this.wholeSlideImageBlock = new WholeSlideImageBlockModel(init.wholeSlideImageBlock); }
            if (init.mediaBlock) { this.mediaBlock = new MediaBlockModel(init.mediaBlock); }
            if (init.questionBlock) { this.questionBlock = new QuestionBlockModel(init.questionBlock); }
            if (init.imageCarouselBlock) { this.imageCarouselBlock = new ImageCarouselBlockModel(init.imageCarouselBlock); }
        }

        if (this.blockType === BlockTypeEnum.Text && !this.textBlock) { this.textBlock = new TextBlockModel(); }
        if (this.blockType === BlockTypeEnum.WholeSlideImage && !this.wholeSlideImageBlock) { this.wholeSlideImageBlock = new WholeSlideImageBlockModel(); }
        if (this.blockType === BlockTypeEnum.Media && !this.mediaBlock) { this.mediaBlock = new MediaBlockModel(); }
        if (this.blockType === BlockTypeEnum.Question && !this.questionBlock) { this.questionBlock = new QuestionBlockModel(undefined); }
        if (this.blockType === BlockTypeEnum.ImageCarousel && !this.imageCarouselBlock) { this.imageCarouselBlock = new ImageCarouselBlockModel(); }
    
    }

    isReadyToPublish(): boolean {
        switch (this.blockType) {
            case BlockTypeEnum.Text:
                return this.textBlock.isReadyToPublish();
            case BlockTypeEnum.WholeSlideImage:
                return this.wholeSlideImageBlock.isReadyToPublish();
            case BlockTypeEnum.Media:
                return this.mediaBlock.isReadyToPublish();
            case BlockTypeEnum.Question:
                return this.questionBlock.isReadyToPublish();
            case BlockTypeEnum.PageBreak:
                return true;
            case BlockTypeEnum.ImageCarousel:
                return this.imageCarouselBlock.isReadyToPublish();
            default:
                return false;
        }
    }

    dispose(): void {
        switch (this.blockType) {
            case BlockTypeEnum.WholeSlideImage:
                this.wholeSlideImageBlock.dispose();
                break;
            case BlockTypeEnum.Media:
                this.mediaBlock.dispose();
                break;
            default:
                break;
        }
    }
}

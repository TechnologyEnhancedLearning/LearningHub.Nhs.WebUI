import { BlockModel } from './blockModel';
import { BlockTypeEnum } from './blockTypeEnum';
import { FileIdModel } from "../files/fileIdModel";
import { MediaBlockModel } from "./mediaBlockModel";
import { AttachmentModel } from "./attachmentModel";
import { TextBlockModel } from './textBlockModel';
import { WholeSlideImageBlockModel } from './wholeSlideImageBlockModel';
import { MediaTypeEnum } from "./mediaTypeEnum";
import { ImageModel } from "./imageModel";
import { QuestionBlockModel } from './questionBlockModel';
import { sortAndReNumberBlocks } from './orderBlocks';
import { VideoMediaModel } from "./videoMediaModel";
import { ImageCarouselBlockModel } from "./imageCarouselBlockModel";

export class BlockCollectionModel {
    blocks: BlockModel[] = [] as BlockModel[];

    constructor(init?: Partial<BlockCollectionModel>) {
        if (init && init.blocks && Array.isArray(init.blocks)) {
            init.blocks.sort((a, b) => a.order - b.order).forEach((block: BlockModel) => {
                this.blocks.push(new BlockModel(block));
            });
            this.sortAndReNumberBlocks();
        }
    }

    static isFirstBlock(block: BlockModel, blocks: BlockModel[]) {
        const index = blocks.indexOf(block);
        return (index === 0);
    }

    static isLastBlock(block: BlockModel, blocks: BlockModel[]) {
        const index = blocks.indexOf(block);
        return (index === (blocks.length - 1));
    }

    isFirstBlock(block: BlockModel): boolean {
        return BlockCollectionModel.isFirstBlock(block, this.blocks);
    }

    isLastBlock(block: BlockModel): boolean {
        return BlockCollectionModel.isLastBlock(block, this.blocks);
    }

    addBlockAtEndOfPage(block: BlockModel, page: number) {
        const lastIndex = this.getEndIndexOfPage(page);
        // Add 0.5 to position it between the last item on the page and the page break that comes after. Blocks are reordered
        // in sortAndReNumberBlocks based on this, and the order will also be corrected to become an integer.
        block.order = lastIndex + 0.5;
        this.blocks.splice(lastIndex + 1, 0, block);
        this.sortAndReNumberBlocks();
    }

    getPageBreakOfPage(pageNumber: number) {
        if (pageNumber === 0) {
            return null;
        } else {
            return this.blocks.filter(block => block.blockType === BlockTypeEnum.PageBreak)[pageNumber - 1];
        }
    }

    getEndIndexOfPage(pageNumber: number) {
        const pageBreaks = this.blocks.filter(block => block.blockType === BlockTypeEnum.PageBreak);
        if (pageNumber === pageBreaks.length) {
            return this.blocks.length;
        } else {
            return this.blocks.indexOf(pageBreaks[pageNumber]) - 1;
        }
    }

    addBlock(blockType: BlockTypeEnum, page = 0) {
        const newBlock = new BlockModel();
        newBlock.blockType = blockType;
        newBlock.order = this.blocks.length;

        switch (blockType) {
            case BlockTypeEnum.Text:
                newBlock.textBlock = new TextBlockModel();
                break;
            case BlockTypeEnum.WholeSlideImage:
                newBlock.wholeSlideImageBlock = new WholeSlideImageBlockModel();
                break;
            case BlockTypeEnum.Question:
                newBlock.questionBlock = new QuestionBlockModel(undefined);
                break;
            case BlockTypeEnum.ImageCarousel:
                newBlock.imageCarouselBlock = new ImageCarouselBlockModel();
                break;
            default:
                break;
        }
        this.addBlockAtEndOfPage(newBlock, page);
        return newBlock;
    }

    addMediaBlock(fileId: number, mediaType: MediaTypeEnum, page = 0) {
        const newBlock = new BlockModel({
            blockType: BlockTypeEnum.Media,
            order: this.blocks.length,
        });
        switch (mediaType) {
            case MediaTypeEnum.Attachment:
                newBlock.mediaBlock = new MediaBlockModel({
                    mediaType: MediaTypeEnum.Attachment,
                    attachment: new AttachmentModel({
                        file: new FileIdModel({
                            fileId: fileId
                        })
                    })
                });
                break;
            case MediaTypeEnum.Image:
                newBlock.mediaBlock = new MediaBlockModel({
                    mediaType: MediaTypeEnum.Image,
                    image: new ImageModel({
                        file: fileId ?
                            new FileIdModel({
                                fileId: fileId
                            }) : undefined
                    })
                });
                break;
            case MediaTypeEnum.Video:
                newBlock.mediaBlock = new MediaBlockModel({
                    mediaType: MediaTypeEnum.Video,
                    video: new VideoMediaModel({
                        file: new FileIdModel({
                            fileId: fileId
                        })
                    })
                });
                break;
            default:
                throw new Error("MediaTypeEnum value not supported. Could not create MediaBlock.");
        }
        this.addBlockAtEndOfPage(newBlock, page);
    }

    duplicateBlock(block: BlockModel) {
        if (this.blockBelongsToThisCollection(block)) {
            const index = this.blocks.indexOf(block);
            const newBlock = new BlockModel(block);
            this.blocks.splice(index + 1, 0, newBlock);
            this.sortAndReNumberBlocks();
        }
    }

    moveBlockUp(block: BlockModel): void {
        if (this.blockBelongsToThisCollection(block)) {
            block.order -= 1.5;
            this.sortAndReNumberBlocks();
        }
    }

    moveBlockDown(block: BlockModel): void {
        if (this.blockBelongsToThisCollection(block)) {
            block.order += 1.5;
            this.sortAndReNumberBlocks();
        }
    }

    deleteBlock(block: BlockModel): void {
        if (this.blockBelongsToThisCollection(block)) {
            block.dispose(); // Pauses any ongoing file uploads
            const index = this.blocks.indexOf(block);
            this.blocks.splice(index, 1);
            this.sortAndReNumberBlocks();
        }
    }

    blockBelongsToThisCollection(block: BlockModel): boolean {
        return (this.blocks.indexOf(block) !== -1);
    }

    sortAndReNumberBlocks(): void {
        sortAndReNumberBlocks(this);
    }

    getPages(): BlockModel[][] {
        return this.blocks.reduceRight(([currentPage, ...previousPages], block) => {
            if (block.blockType === BlockTypeEnum.PageBreak) {
                return [[], currentPage, ...previousPages];
            } else {
                return [[block, ...currentPage], ...previousPages];
            }
        }, [[]] as BlockModel[][]);
    }

    getBlocksByPage(page: number): BlockModel[] {
        return this.getPages()[page];
    }

    deletePage(page: number) {
        const blocksToDelete = [
            ...this.getBlocksByPage(page),
            // The first page never has a page break, so delete the page break of the second page instead
            this.getPageBreakOfPage(Math.max(1, page))
        ];

        for (const block of blocksToDelete) {
            this.deleteBlock(block);
        }
    }

    isPageReady(page: number): boolean {
        const pageBlocks = this.getBlocksByPage(page);
        return pageBlocks.length > 0 && pageBlocks.every(block => block.isReadyToPublish());
    }

    isReadyToPublish() {
        return (
            this.blocks.length > 0 &&
            this.blocks.every(block => block.isReadyToPublish()) &&
            this.getPages().every(page => page.length > 0)
        );
    }

    missingSlides(): boolean {
        const wholeSlideImageBlocks = this.blocks.filter(block => block.blockType === BlockTypeEnum.WholeSlideImage);
        return wholeSlideImageBlocks.some(wholeSlideImageBlock => wholeSlideImageBlock.wholeSlideImageBlock.missingSlides());
    }
};

import { WholeSlideImageModel } from './wholeSlideImageModel';
import { WholeSlideImageBlockItemModel } from './wholeSlideImageBlockItemModel';

export class WholeSlideImageBlockModel {
    wholeSlideImageBlockItems: WholeSlideImageBlockItemModel[] = [] as WholeSlideImageBlockItemModel[];

    constructor(init?: Partial<WholeSlideImageBlockModel>) {
        if (init && init.wholeSlideImageBlockItems && Array.isArray(init.wholeSlideImageBlockItems)) {
            init.wholeSlideImageBlockItems.forEach((wholeSlideImageBlockItem: WholeSlideImageBlockItemModel) => {
                this.wholeSlideImageBlockItems.push(new WholeSlideImageBlockItemModel(wholeSlideImageBlockItem));
            });

            this.sortAndReNumberSlides();
        }
    }

    isFirstSlide(wholeSlideImageBlockItem: WholeSlideImageBlockItemModel): boolean {
        return (this.wholeSlideImageBlockItems.indexOf(wholeSlideImageBlockItem) === 0);
    }

    isLastSlide(wholeSlideImageBlockItem: WholeSlideImageBlockItemModel): boolean {
        return (this.wholeSlideImageBlockItems.indexOf(wholeSlideImageBlockItem)
            === (this.wholeSlideImageBlockItems.length - 1));
    }

    addSlide(fileId?: number): WholeSlideImageBlockItemModel {
        const wholeSlideImageBlockItem = new WholeSlideImageBlockItemModel({wholeSlideImage: new WholeSlideImageModel()});
        if (fileId !== undefined) {
            wholeSlideImageBlockItem.setFileId(fileId);
        }
        wholeSlideImageBlockItem.order = this.wholeSlideImageBlockItems.length;
        this.wholeSlideImageBlockItems.push(wholeSlideImageBlockItem);
        return wholeSlideImageBlockItem;
    }

    moveSlideUp(wholeSlideImageBlockItem: WholeSlideImageBlockItemModel): void {
        if (this.slideBelongsToThisBlock(wholeSlideImageBlockItem)) {
            wholeSlideImageBlockItem.order -= 1.5;
            this.sortAndReNumberSlides();
        }
    }

    moveSlideDown(wholeSlideImageBlockItem: WholeSlideImageBlockItemModel): void {
        if (this.slideBelongsToThisBlock(wholeSlideImageBlockItem)) {
            wholeSlideImageBlockItem.order += 1.5;
            this.sortAndReNumberSlides();
        }
    }

    deleteSlide(wholeSlideImageBlockItem: WholeSlideImageBlockItemModel): void {
        if (this.slideBelongsToThisBlock(wholeSlideImageBlockItem)) {
            wholeSlideImageBlockItem.dispose(); // Pauses any ongoing file uploads
            const index = this.wholeSlideImageBlockItems.indexOf(wholeSlideImageBlockItem);
            this.wholeSlideImageBlockItems.splice(index, 1);
            this.sortAndReNumberSlides();
        }
    }

    slideBelongsToThisBlock(wholeSlideImageBlockItem: WholeSlideImageBlockItemModel): boolean {
        return (this.wholeSlideImageBlockItems.indexOf(wholeSlideImageBlockItem) !== -1);
    }

    sortAndReNumberSlides(): void {
        this.wholeSlideImageBlockItems.sort((slideA, slideB) => slideA.order - slideB.order);
        this.wholeSlideImageBlockItems.forEach((slideEl, slideIndex) => slideEl.order = slideIndex);
    }

    isReadyToPublish(): boolean {
        return this.wholeSlideImageBlockItems.length > 0 &&
            this.wholeSlideImageBlockItems.every(wholeSlideImageBlockItem => wholeSlideImageBlockItem.isReadyToPublish());
    }

    missingSlides(): boolean {
        return this.wholeSlideImageBlockItems.some(wholeSlideImageBlockItem => !wholeSlideImageBlockItem.wholeSlideImage.getFileModel());
    }

    dispose(): void {
        this.wholeSlideImageBlockItems.forEach(wholeSlideImageBlockItem => wholeSlideImageBlockItem.dispose());
    }
};

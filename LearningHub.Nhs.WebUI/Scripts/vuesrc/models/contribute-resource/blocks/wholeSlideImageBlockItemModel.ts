import { WholeSlideImageModel } from './wholeSlideImageModel';

export class WholeSlideImageBlockItemModel {
    order: number = undefined;
    wholeSlideImage: WholeSlideImageModel = undefined;
    placeholderText: string = undefined;
    
    constructor(init?: Partial<WholeSlideImageBlockItemModel>) {
        if (init) {
            this.order = init.order;
            this.wholeSlideImage = new WholeSlideImageModel(init.wholeSlideImage);
            this.placeholderText = init.placeholderText;
        }
    }

    setFileId(fileId: number): void {
        this.wholeSlideImage.setFileId(fileId);
    }

    isReadyToPublish(): boolean {
        return this.wholeSlideImage.isReadyToPublish() || (typeof this.placeholderText === "string" && this.placeholderText.length > 0);
    }

    dispose(): void {
        this.wholeSlideImage.dispose();
    }

    isPlaceholderWithoutTitle(): boolean {
        return  !this.wholeSlideImage.getFileModel() && 
                (typeof this.wholeSlideImage.title !== "string" || this.wholeSlideImage.title.length === 0)
    }
}
import {BlockCollectionModel} from "./blockCollectionModel";
import {BlockTypeEnum} from "./blockTypeEnum";

export class ImageCarouselBlockModel {
    description: string = undefined;
    imageBlockCollection: BlockCollectionModel = undefined;

    constructor(init?: Partial<ImageCarouselBlockModel>) {
        if (init) {
            this.description = init.description as string;
            this.imageBlockCollection = new BlockCollectionModel(init.imageBlockCollection);
        } else {
            this.imageBlockCollection = new BlockCollectionModel();
        }
    }

    isReadyToPublish(): boolean {
        return this.imageBlockCollection &&
            !!this.description &&
            this.imageBlockCollection.isReadyToPublish();
    }
}
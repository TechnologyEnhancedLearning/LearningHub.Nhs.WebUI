import { BlockCollectionModel } from './blocks/blockCollectionModel';

export class CaseResourceModel {
    resourceVersionId: number = 0;
    blockCollection: BlockCollectionModel;
    
    constructor(init?: Partial<CaseResourceModel>) {
        if (init) {
            this.resourceVersionId = init.resourceVersionId;
            this.blockCollection = new BlockCollectionModel(init.blockCollection);
        }
    }

    isReadyToPublish(): boolean {
        return this.blockCollection.isReadyToPublish();
    }
}

import { WholeSlideImageFileStatusEnum } from './wholeSlideImageFileStatusEnum';

export class WholeSlideImageFileModel {
    status: WholeSlideImageFileStatusEnum;
    processingErrorMessage: string;
    width: number;
    height: number;
    deepZoomTileSize: number;
    deepZoomOverlap: number;
    layers: number;

    constructor(init?: Partial<WholeSlideImageFileModel>) {
        if (init) {
            this.status = init.status;
            this.processingErrorMessage = init.processingErrorMessage;
            this.width = init.width;
            this.height = init.height;
            this.deepZoomTileSize = init.deepZoomTileSize;
            this.deepZoomOverlap = init.deepZoomOverlap;
            this.layers = init.layers;
        }
    }
};

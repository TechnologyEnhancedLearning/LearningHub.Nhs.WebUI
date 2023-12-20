import { ImageAnnotationMarkTypeEnum } from './ImageAnnotationMarkTypeEnum';
import { ImageAnnotationMarkFreehandShapeData } from './imageAnnotationMarkFreehandShapeData';

export class ImageAnnotationMarkModel {
    type: ImageAnnotationMarkTypeEnum = undefined;
    markLabel: string = undefined;
    freehandMarkShapeData: ImageAnnotationMarkFreehandShapeData = undefined;

    constructor(init?: Partial<ImageAnnotationMarkModel>) {
        if (init) {
            this.type = init.type as ImageAnnotationMarkTypeEnum;
            this.markLabel = init.markLabel as string;
            this.freehandMarkShapeData = new ImageAnnotationMarkFreehandShapeData(init.freehandMarkShapeData);
        }
    }
}
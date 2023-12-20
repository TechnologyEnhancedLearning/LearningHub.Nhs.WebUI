import { ImageAnnotationColourEnum } from './imageAnnotationColourEnum';
import { ImageAnnotationMarkModel } from "./imageAnnotationMarkModel";

export class ImageAnnotation {
    order: number = undefined;
    label: string = undefined;
    description: string = undefined;
    pinXCoordinate: number = 0;
    pinYCoordinate: number = 0;
    colour: ImageAnnotationColourEnum = ImageAnnotationColourEnum.Black;
    imageAnnotationMarks: ImageAnnotationMarkModel[] = [] as ImageAnnotationMarkModel[];

    constructor(init?: Partial<ImageAnnotation>) {
        if (init) {
            this.order = init.order as number;
            this.label = init.label as string;
            this.description = init.description as string;
            this.pinXCoordinate = init.pinXCoordinate as number;
            this.pinYCoordinate = init.pinYCoordinate as number;
            this.colour = init.colour as ImageAnnotationColourEnum;
            this.imageAnnotationMarks = init.imageAnnotationMarks as ImageAnnotationMarkModel[];
        }
    }

    deleteAnnotationMark(wholeSlideImageMark: ImageAnnotationMarkModel): void {
        if (this.markBelongsToThisAnnotation(wholeSlideImageMark)) {
            const index = this.imageAnnotationMarks.indexOf(wholeSlideImageMark);
            this.imageAnnotationMarks.splice(index, 1);
        }
    }

    markBelongsToThisAnnotation(wholeSlideImageMark: ImageAnnotationMarkModel): boolean {
        return this.imageAnnotationMarks.includes(wholeSlideImageMark);
    }
}
import { PathCoordinates } from './pathCoordinates';

export class ImageAnnotationMarkFreehandShapeData {
    pathCoordinates: PathCoordinates[] = [] as PathCoordinates[];

    constructor(init?: Partial<ImageAnnotationMarkFreehandShapeData>) {
        if (init) {
            this.pathCoordinates = init.pathCoordinates as PathCoordinates[]
        }
    }
}
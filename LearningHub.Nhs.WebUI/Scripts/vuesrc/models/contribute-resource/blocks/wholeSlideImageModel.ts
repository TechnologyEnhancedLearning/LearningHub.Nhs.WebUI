import { FileIdModel } from '../files/fileIdModel';
import { FileModel } from '../files/fileModel';
import { FileStore } from '../files/fileStore';
import { WholeSlideImageFileStatusEnum } from '../files/wholeSlideImageFileStatusEnum';
import { ImageAnnotation } from "./annotations/imageAnnotationModel";

let nextWholeSlideImageRef = 0;

export class WholeSlideImageModel {
    title: string = undefined;
    file: FileIdModel = undefined;
    annotations: ImageAnnotation[] = [] as ImageAnnotation[];
    wholeSlideImageRef: number = null;
    // Whole Slide Images need this bool in order to update upon the image uploading.
    readyToPublish: boolean = undefined;


    constructor(init?: Partial<WholeSlideImageModel>) {
        this.wholeSlideImageRef = nextWholeSlideImageRef++;

        if (init) {
            this.title = init.title;

            this.file = new FileIdModel();
            if (init.file) {
                this.file.fileId = init.file.fileId;
            }
            if (init.file as FileModel) {
                FileStore.addFile(new FileModel(init.file));
            }

            if (init.annotations) {
                this.annotations = init.annotations.map(a => new ImageAnnotation(a));
                this.sortAnnotations();
            }
        }
    }

    getFileModel(): FileModel {
        if (!this.file) {
            return undefined;
        }

        return FileStore.getFile(this.file.fileId);
    }

    setFileId(fileId: number): void {
        // Add a FileIdModel to this WholeSlideImageModel if this is a new file
        if (!this.file || this.file.fileId !== fileId) {
            this.file = new FileIdModel({ fileId: fileId });
        }
    }

    addAnnotation(): ImageAnnotation {
        const imageAnnotation = new ImageAnnotation();
        imageAnnotation.order = this.annotations.length;
        this.annotations.push(imageAnnotation);
        return imageAnnotation;
    }

    moveAnnotationUp(imageAnnotation: ImageAnnotation): void {
        if (this.annotationBelongsToThisWsi(imageAnnotation)) {
            imageAnnotation.order -= 1.5;
            this.sortAndReNumberAnnotations();
        }
    }

    moveAnnotationDown(imageAnnotation: ImageAnnotation): void {
        if (this.annotationBelongsToThisWsi(imageAnnotation)) {
            imageAnnotation.order += 1.5;
            this.sortAndReNumberAnnotations();
        }
    }

    deleteAnnotation(imageAnnotation: ImageAnnotation): void {
        if (this.annotationBelongsToThisWsi(imageAnnotation)) {
            const index = this.annotations.indexOf(imageAnnotation);
            this.annotations.splice(index, 1);
            this.sortAndReNumberAnnotations();
        }
    }

    annotationBelongsToThisWsi(imageAnnotation: ImageAnnotation): boolean {
        return (this.annotations.indexOf(imageAnnotation) !== -1);
    }

    sortAndReNumberAnnotations(): void {
        this.sortAnnotations();
        this.annotations.forEach((aEl, aIndex) => aEl.order = aIndex);
    }

    sortAnnotations(): void {
        this.annotations.sort((a1, a2) => a1.order - a2.order);
    }

    getIsReadyToPublish(): boolean {
        return this.getFileModel() &&
            this.getFileModel().wholeSlideImageFile &&
            this.getFileModel().wholeSlideImageFile.status === WholeSlideImageFileStatusEnum.ProcessingComplete;
    }

    updatePublishingStatus(): void {
        this.readyToPublish = this.getIsReadyToPublish();
    }

    isReadyToPublish(): boolean {
        this.readyToPublish = this.getIsReadyToPublish();
        return this.readyToPublish;
    }

    dispose() {
        if (this.getFileModel()) {
            this.getFileModel().pauseUpload();
        }
    }
};

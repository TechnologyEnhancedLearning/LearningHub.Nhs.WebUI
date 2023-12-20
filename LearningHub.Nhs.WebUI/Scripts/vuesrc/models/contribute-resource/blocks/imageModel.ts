import { FileIdModel } from "../files/fileIdModel";
import { FileModel } from "../files/fileModel";
import { FileStore } from "../files/fileStore";

export class ImageModel {
    file: FileIdModel = undefined;
    altText: string = undefined;
    description: string = undefined;

    constructor(init?: Partial<ImageModel>) {
        if (init) {
            this.file = new FileIdModel();
            if (init.file) {
                this.file.fileId = init.file.fileId
            }
            if (init.file as FileModel) {
                FileStore.addFile(new FileModel(init.file));
            }
            this.altText = init.altText;
            this.description = init.description;
        }
    }

    getFileModel(): FileModel {
        if (!this.file) {
            return undefined;
        }

        return FileStore.getFile(this.file.fileId);
    }

    setFileId(fileId: number): void {
        // Add a FileIdModel to this ImageModel if this is a new file
        if (!this.file || this.file.fileId !== fileId) {
            this.file = new FileIdModel({ fileId });
        }
    }

    isReadyToPublish(): boolean {
        return this.getFileModel()
            && this.getFileModel().isUploadComplete()
            && !!this.altText;
    }

    dispose() {
        if (this.getFileModel()) {
            this.getFileModel().pauseUpload();
        }
    }
}

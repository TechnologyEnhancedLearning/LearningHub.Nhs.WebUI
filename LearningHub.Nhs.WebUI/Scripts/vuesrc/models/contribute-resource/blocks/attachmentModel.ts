import { FileIdModel } from "../files/fileIdModel";
import { FileModel } from "../files/fileModel";
import { FileStore } from "../files/fileStore";
import {FileUploadState} from "../../../helpers/fileUpload";

export class AttachmentModel {
    file: FileIdModel = undefined;
    
    constructor(init?: Partial<AttachmentModel>) {
        if (init) {
            this.file = new FileIdModel();
            if (init.file) {
                this.file.fileId = init.file.fileId
            }
            if (init.file as FileModel) {
                FileStore.addFile(new FileModel(init.file));
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
        // Add a FileIdModel to this AttachmentModel if this is a new file
        if (!this.file || this.file.fileId !== fileId) {
            this.file = new FileIdModel({ fileId: fileId });
        }
    }

    isReadyToPublish(): boolean {
        return this.getFileModel()
            && this.getFileModel().isUploadComplete();
    }

    dispose() {
        if (this.getFileModel()) {
            this.getFileModel().pauseUpload();
        }
    }
}

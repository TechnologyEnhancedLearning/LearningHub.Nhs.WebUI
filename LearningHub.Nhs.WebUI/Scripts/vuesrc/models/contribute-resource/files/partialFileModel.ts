
export class PartialFileModel {
    uploadedFileSize: number = undefined;
    totalFileSize: number = undefined;

    constructor(init?: Partial<PartialFileModel>) {
        if (init) {
            this.uploadedFileSize = init.uploadedFileSize;
            this.totalFileSize = init.totalFileSize;
        }
    }
};

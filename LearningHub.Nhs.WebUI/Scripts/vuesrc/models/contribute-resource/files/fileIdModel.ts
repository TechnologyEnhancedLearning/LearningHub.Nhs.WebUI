
export class FileIdModel {
    fileId: number = undefined;

    constructor(init?: Partial<FileIdModel>) {
        if (init) {
            this.fileId = init.fileId;
        }
    }
}

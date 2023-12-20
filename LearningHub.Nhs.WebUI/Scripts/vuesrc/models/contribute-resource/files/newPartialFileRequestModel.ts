import { FileUploadPostProcessingOptions } from '../../../helpers/fileUpload';

export class NewPartialFileRequestModel {
    fileName: string = undefined;
    totalFileSize: number = undefined;
    postProcessingOptions: FileUploadPostProcessingOptions = undefined;

    constructor(init?: Partial<NewPartialFileRequestModel>) {
        if (init) {
            this.fileName = init.fileName;
            this.totalFileSize = init.totalFileSize;
            this.postProcessingOptions = init.postProcessingOptions;
        }
    }
};

export class FileUploadSettingsModel {
    fileUploadSizeLimit: number;
    fileUploadSizeLimitText: string;
    allowedThreads: number;
    chunkSize: number;
    timeoutSec: number;
}

export class ContributeSettingsModel {
    fileUploadSettings: FileUploadSettingsModel = new FileUploadSettingsModel();
}

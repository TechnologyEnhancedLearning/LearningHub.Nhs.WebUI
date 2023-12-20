import { CaptionsFileModel } from "./CaptionsFileModel";
import { TranscriptFileModel } from "./TranscriptFileModel";
import { VideoFileStatusEnum } from "../files/VideoFileStatusEnum";

export class VideoFileModel {
    captionsFile: CaptionsFileModel = undefined;
    transcriptFile: TranscriptFileModel = undefined;

    status: VideoFileStatusEnum = undefined;
    processingErrorMessage: string = undefined;
    azureAssetOutputFilePath: string = undefined;
    locatorUri: string = undefined;
    encodeJobName: string = undefined;
    durationInMilliseconds: number = undefined;

    constructor(init?: Partial<VideoFileModel>) {
        if (init) {
            this.captionsFile = init.captionsFile ? new CaptionsFileModel(init.captionsFile) : undefined;
            this.transcriptFile = init.transcriptFile ? new TranscriptFileModel(init.transcriptFile) : undefined;

            this.status = init.status;
            this.processingErrorMessage = init.processingErrorMessage;
            this.azureAssetOutputFilePath = init.azureAssetOutputFilePath;
            this.locatorUri = init.locatorUri;
            this.encodeJobName = init.encodeJobName;
            this.durationInMilliseconds = init.durationInMilliseconds;
        }
    }
}
import { PartialFileModel } from './partialFileModel';
import { VideoFileModel } from '../blocks/videoFileModel';
import { VideoFileStatusEnum } from './VideoFileStatusEnum';
import { WholeSlideImageFileModel } from './wholeSlideImageFileModel';
import { WholeSlideImageFileStatusEnum } from './wholeSlideImageFileStatusEnum';
import {
    FileUpload,
    FileUploadState,
    FileUploadType,
    getFileExtensionAllowedList,
    getFileExtensionBlockedList,
    getMaxFileSize,
} from '../../../helpers/fileUpload';
import { isIncludedInListIgnoringCase } from "../../../helpers/utils";

export class FileModel {
    fileId: number = undefined;
    fileName: string = undefined;
    filePath: string = undefined;
    fileSizeKb: number = undefined;
    partialFile: PartialFileModel = undefined;
    videoFile: VideoFileModel = undefined;
    wholeSlideImageFile: WholeSlideImageFileModel = undefined;

    fileUpload: FileUpload = undefined;

    constructor(init?: Partial<FileModel>) {
        if (init) {
            this.fileId = init.fileId;
            this.fileName = init.fileName;
            this.filePath = init.filePath;
            this.fileSizeKb = init.fileSizeKb;
            if (init.partialFile) { this.partialFile = new PartialFileModel(init.partialFile); }
            if (init.videoFile) { this.videoFile = new VideoFileModel(init.videoFile); }
            if (init.wholeSlideImageFile) { this.wholeSlideImageFile = new WholeSlideImageFileModel(init.wholeSlideImageFile); }
        }
    }

    shouldPollForChanges(): boolean {
        // The filePath always comes from this polling
        // If the filePath has been set, then we know that at least one poll has completed, so other structures should be present
        // (e.g. wholeSlideImageFile for whole-slide images)
        const wantInitialUpdateFromServer = !this.filePath;

        const uploadIsComplete =
            !this.partialFile ||
            (this.partialFile.uploadedFileSize === this.partialFile.totalFileSize && this.partialFile.totalFileSize > 0);

        const wantWholeSlideImageData =
            this.wholeSlideImageFile /* after the initial update from the server, this.wholeSlideImageFile will have a value */ &&
            uploadIsComplete /* the whole slide image data will only change once the upload is complete */ &&
            (this.wholeSlideImageFile.status !== WholeSlideImageFileStatusEnum.ProcessingComplete &&
                this.wholeSlideImageFile.status !== WholeSlideImageFileStatusEnum.ProcessingFailed);

        const wantVideoData =
            this.videoFile &&
            uploadIsComplete &&
            (this.videoFile.status !== VideoFileStatusEnum.ProcessingComplete &&
                this.videoFile.status !== VideoFileStatusEnum.ProcessingFailed);

        return wantInitialUpdateFromServer ||
            wantWholeSlideImageData ||
            wantVideoData;
    }

    updateFromPolling(newFile: FileModel): void {
        // No need to update:
        // * fileId, because that's the key we used to know which model to update
        // * partialFile, because that's updated more frequently by the FileUpload widget
        this.fileName = newFile.fileName;
        this.filePath = newFile.filePath;
        this.fileSizeKb = newFile.fileSizeKb;

        if (newFile.wholeSlideImageFile) {
            this.wholeSlideImageFile = new WholeSlideImageFileModel(newFile.wholeSlideImageFile);
        }

        if (newFile.videoFile) {
            this.videoFile = new VideoFileModel(newFile.videoFile);
        }
    }

    pauseUpload(): void {
        if (this.fileUpload) {
            this.fileUpload.pause();
        }
    }

    hasOngoingFileUpload(): boolean {
        return this.fileUpload && this.fileUpload.state === FileUploadState.Uploading;
    }

    isNotAllowedToUpload(fileCategory: FileUploadType): boolean {
        return this.isTooLargeForUpload() || this.isWrongType(fileCategory);
    }

    isTooLargeForUpload(): boolean {
        return this.partialFile && this.partialFile.totalFileSize > getMaxFileSize();
    }

    isWrongType(fileCategory: FileUploadType): boolean {
        return this.excludedFromAllowedList(fileCategory)
            || this.includedInBlockedList()
    }
    
    excludedFromAllowedList(fileCategory: FileUploadType): boolean {
        const allowedList = getFileExtensionAllowedList(fileCategory);
        return allowedList.length > 0 &&
            !this.includedInList(getFileExtensionAllowedList(fileCategory));
    }

    includedInBlockedList(): boolean {
        const blockedList = getFileExtensionBlockedList();
        return blockedList.length > 0 &&
            this.includedInList(blockedList);
    }

    getFileExtension(): string {
        const lastDot = this.fileName.lastIndexOf('.');
        return this.fileName.substring(lastDot);
    }

    includedInList(list: string[]): boolean {
        const fileExtension = this.getFileExtension();
        return isIncludedInListIgnoringCase(list, fileExtension);
    }

    isUploadComplete(): boolean {
        const haveDataFromServer = !!this.filePath;
        const noPartialFile = !this.partialFile;
        const fileUploadSucceeded = this.fileUpload && this.fileUpload.state === FileUploadState.Succeeded;
        
        return haveDataFromServer && (noPartialFile || fileUploadSucceeded);
    }
    
    getDownloadResourceLink(): string {
        return `/api/resource/DownloadResource`
            + `?filePath=${encodeURIComponent(this.filePath)}`
            + `&fileName=${encodeURIComponent(this.fileName)}`
    }
}

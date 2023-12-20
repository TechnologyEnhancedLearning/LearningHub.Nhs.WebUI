import * as tus from "tus-js-client";
import ContributeApi from '../contribute-resource/contributeApi';
import { FileModel } from '../models/contribute-resource/files/fileModel';
import { PartialFileModel } from '../models/contribute-resource/files/partialFileModel';
import { FileStore } from '../models/contribute-resource/files/fileStore';
import { NewPartialFileRequestModel } from '../models/contribute-resource/files/newPartialFileRequestModel';
import { WholeSlideImageFileModel } from "../models/contribute-resource/files/wholeSlideImageFileModel";
import { WholeSlideImageFileStatusEnum } from "../models/contribute-resource/files/wholeSlideImageFileStatusEnum";
import { FileSizeHelper } from './fileSizeHelper';
import { MediaTypeEnum } from "../models/contribute-resource/blocks/mediaTypeEnum";
import { VideoFileStatusEnum } from "../models/contribute-resource/files/VideoFileStatusEnum";
import { VideoFileModel } from "../models/contribute-resource/blocks/videoFileModel";
import { isIncludedInListIgnoringCase } from "./utils";

export enum FileUploadPostProcessingOptions {
    None = 0,
    WholeSlideImage = 1,
    Video = 2,
}

export enum FileUploadType {
    None = 0,
    WholeSlideImage = 1,
    Attachment = 2,
    Image = 3,
    Video = 4,
    Audio = 5,
    Media = 6, // Covers image, video and audio
    RestrictedImage = 7 // Restricts .tif, .tiff, .psd
}

export class FileUploadOptions {
    file: File;
    fileModel: FileModel;
    onError: (error: any) => void;
    onProgress: (bytesUploaded: number, bytesTotal: number) => void;
    onSuccess: () => void;
}

export enum FileUploadState {
    NotStarted,
    Uploading,
    Paused,
    Succeeded,
    Failed,
}

const MAX_FILE_SIZE = 10 * 1000 * 1000 * 1000; // 10GB

// This list should correspond to the disallowed extensions contained in the FileType table
const BLOCKED_FILE_EXTENSIONS = ['.app', '.asp', '.aspx', '.dll', '.dmg', '.exe', '.flv', '.f4v', '.js', '.jsp',
    '.php', '.shtm', '.shtml', '.swf'];

const IMAGE_FILE_EXTENSIONS = ['.apng', '.avif', '.bmp', '.cur', '.gif', '.ico', '.jfif', '.jpeg', '.jpg', '.pjp',
    '.pjpeg', '.png', '.psd', '.svg', '.tif', '.tiff', '.webp'];
const RESTRICTED_IMAGE_FILE_EXTENSIONS = ['.apng', '.bmp', '.cur', '.gif', '.jfif', '.jpeg', '.jpg', '.pjp',
    '.pjpeg', '.png', '.svg', '.webp'];
const VIDEO_FILE_EXTENSIONS = ['.3gp', '.3gpp', '.asf', '.avi', '.dvr-ms', '.gxf', '.flv', '.ismv', '.m4v', '.mkv',
    '.mov', '.mp4', '.mpg', '.mxf', '.wmv'];
const AUDIO_FILE_EXTENSIONS = ['.aac', '.flac', '.m4a', '.mp3', '.wav', '.wma'];
const ATTACHMENT_FILE_EXTENSIONS = ['.dhtml', '.doc', '.docx', '.htm', '.html', '.key', '.numbers', '.odp', '.ods',
    '.odt', '.pages', '.pdf', '.ppt', '.pptx', '.xlm', '.xls', '.xlsm', '.xlsx', '.zip'];
const WSI_FILE_EXTENSIONS = ['.ndpi', '.tif', '.tiff', '.svs', '.zip'];

const MEDIA_FILE_EXTENSIONS = [
    ...IMAGE_FILE_EXTENSIONS,
    ...VIDEO_FILE_EXTENSIONS,
    ...AUDIO_FILE_EXTENSIONS,
    ...ATTACHMENT_FILE_EXTENSIONS
].sort();

const ALLOWED_FILE_EXTENSIONS: { [category: number]: string[] } = {
    [FileUploadType.WholeSlideImage]: WSI_FILE_EXTENSIONS,
    [FileUploadType.Image]: IMAGE_FILE_EXTENSIONS,
    [FileUploadType.RestrictedImage]: RESTRICTED_IMAGE_FILE_EXTENSIONS,
    [FileUploadType.Video]: VIDEO_FILE_EXTENSIONS,
    [FileUploadType.Audio]: AUDIO_FILE_EXTENSIONS,
    [FileUploadType.Media]: MEDIA_FILE_EXTENSIONS,
};

const UPLOAD_ENDPOINT: string = '/api/file-uploads/'; // Endpoint is the upload creation URL from your tus server
const UPLOAD_CHUNK_SIZE: number = 1 * 1000 * 1000; // 1MB
const UPLOAD_RETRY_DELAYS: number[] = []; //[0, 3000, 5000, 10000, 20000]; // Retry delays will enable tus-js-client to automatically retry on errors

export class FileUpload {

    state: FileUploadState = FileUploadState.NotStarted;

    private file: File;
    private fileModel: FileModel;
    private onError: (error: any) => void;
    private onProgress: (bytesUploaded: number, bytesTotal: number) => void;
    private onSuccess: () => void;

    private upload: tus.Upload;

    constructor(options: Partial<FileUploadOptions>) {
        Object.assign(this, options);

        this.fileModel.fileUpload = this;
    }

    start(): void {
        this.upload = new tus.Upload(this.file, {
            uploadUrl: UPLOAD_ENDPOINT + this.fileModel.fileId,
            chunkSize: UPLOAD_CHUNK_SIZE,
            retryDelays: UPLOAD_RETRY_DELAYS,

            // Callback for errors which cannot be fixed using retries
            onError: (error: any) => {
                this.state = FileUploadState.Failed;
                this.onError && this.onError(error);
            },

            // Callback for reporting upload progress
            onProgress: (bytesUploaded: number, bytesTotal: number) => {
                this.fileModel.fileName = this.getFileName();
                this.fileModel.partialFile.uploadedFileSize = bytesUploaded;
                this.fileModel.partialFile.totalFileSize = bytesTotal;
                this.fileModel.fileUpload = this;

                this.onProgress && this.onProgress(bytesUploaded, bytesTotal);
            },

            // Callback for once the upload is completed
            onSuccess: () => {
                this.state = FileUploadState.Succeeded;
                this.onSuccess && this.onSuccess();
            }
        });

        // Start the upload
        this.upload.start();

        this.state = FileUploadState.Uploading;
    }

    pause(): void {
        if (this.state === FileUploadState.Uploading) {
            this.upload.abort();
            this.state = FileUploadState.Paused;
        }
    }

    resume(): void {
        if (this.state === FileUploadState.Paused) {
            this.upload.start();
            this.state = FileUploadState.Uploading;
        }
    }

    getFileId(): number {
        const url = this.upload.url;
        const lastSlashPosition = url.lastIndexOf('/');
        const fileId = parseInt(url.substring(lastSlashPosition + 1));
        return fileId;
    }

    getFileName(): string {
        return (this.upload.file as File).name;
    }
}

export const startFileUpload = async function (file: File, fileType: FileUploadType, callbackUpdateProgress?: () => void): Promise<number> {
    const postProcessingOptions = getPostProcessingOptionEnumFromFileUploadType(fileType);
    const partialFileModel = await ContributeApi.createPartialFile(new NewPartialFileRequestModel({
        fileName: file.name,
        totalFileSize: file.size,
        postProcessingOptions: postProcessingOptions,
    }));

    const fileId: number = partialFileModel.fileId;

    const fileModel = new FileModel({
        fileId: fileId,
        fileName: file.name,
        partialFile: new PartialFileModel({
            totalFileSize: file.size,
        }),
    });
    addPostProcessingParts(fileModel, postProcessingOptions);
    FileStore.addFile(fileModel);

    const upload = new FileUpload({
        file: file,
        fileModel: fileModel,

        // Callback for errors which cannot be fixed using retries
        onError: (error: any) => {
            console.log("Failed because: " + error);
            callbackUpdateProgress && callbackUpdateProgress();
        },
    
        onProgress: () => {
            callbackUpdateProgress && callbackUpdateProgress();
        },

        onSuccess: () => {
            callbackUpdateProgress && callbackUpdateProgress();
        }
    });

    if (fileModel.isNotAllowedToUpload(fileType)) {
        upload.state = FileUploadState.Failed;
    }

    if (upload.state !== FileUploadState.Failed) {
        upload.start();
    }

    return fileId;
};

export const resumeFileUpload = function (file: File, fileModel: FileModel, callbackUpdateProgress?: () => void): void {
    const upload = new FileUpload({
        file: file,
        fileModel: fileModel,

        // Callback for errors which cannot be fixed using retries
        onError: (error: any) => {
            console.log("Failed because: " + error);
            callbackUpdateProgress && callbackUpdateProgress();
        },
    
        onProgress: () => {
            callbackUpdateProgress && callbackUpdateProgress();
        },
    
        onSuccess: () => {
            callbackUpdateProgress && callbackUpdateProgress();
        }
    });

    upload.start();
};

export const startUploadsFromFileElement = async function (target: HTMLInputElement, callback: (fileId: number, mediaType: MediaTypeEnum) => void) {
    if (target.value !== '') {
        for (let i = 0; i < target.files.length; i++) {
            const file = target.files[i] as File;
            const fileExtension = file.name.split('.').pop();
            const mediaType = getMediaTypeFromFileExtension(`.${ fileExtension }`);
            const fileUploadType = getUploadTypeFromMediaType(mediaType);
            const fileId = await startFileUpload(file, fileUploadType);
            callback(fileId, mediaType);
        }

        // Empty the list of files
        // The "onchange" event only fires if the list of files "changes"
        // So, it doesn't fire if you select the same file twice
        // To allow this, we empty the list of files after each one if chosen
        target.value = ''; // Empties the list of files - the only value you can set a <input type="file">.value to is '' (the empty string)
    }
};

export const getFileExtensionAllowedList = function (fileCategory: FileUploadType): string[] {
    const extensions = ALLOWED_FILE_EXTENSIONS[fileCategory] || [];
    const cloneOfArray = extensions.slice();
    return cloneOfArray;
};

export const getFileExtensionAllowedListFormatted = function (fileCategory: FileUploadType): string {
    const extensions = getFileExtensionAllowedList(fileCategory);
    const lastExtension = extensions.pop();

    let extensionsString = '';
    if (extensions.length > 0) {
        extensionsString = extensions.join(' ') + ' and ' + lastExtension;
    } else {
        extensionsString = lastExtension;
    }
    return extensionsString;
};

// This is for use in <input> elements if we want to specifically define what file types are allowed
// via the element's `accept` property
export const getAllowedFileExtensionsInAcceptFormat = function (fileCategory: FileUploadType): string {
    return getFileExtensionAllowedList(fileCategory).join(',');
};

export const getFileExtensionBlockedList = function (): string[] {
    return BLOCKED_FILE_EXTENSIONS.slice();
};

export const getFileExtensionBlockListFormatted = function (): string {
    const extensions = getFileExtensionBlockedList();
    const lastExtension = extensions.pop();

    let extensionsString = '';
    if (extensions.length > 0) {
        extensionsString = extensions.join(' ') + ' and ' + lastExtension;
    } else {
        extensionsString = lastExtension;
    }
    return extensionsString;
};

export const getMaxFileSize = function (): number {
    return MAX_FILE_SIZE;
};

export const getMaxFileSizeFormatted = function (): string {
    return FileSizeHelper.getFormattedFileSize(MAX_FILE_SIZE);
};

export const getPostProcessingOptionEnumFromFileUploadType = function (fileType: FileUploadType):
    FileUploadPostProcessingOptions {
    switch (fileType) {
        case FileUploadType.WholeSlideImage:
            return FileUploadPostProcessingOptions.WholeSlideImage;
        case FileUploadType.Video:
            return FileUploadPostProcessingOptions.Video;
        default:
            return FileUploadPostProcessingOptions.None;
    }
};

export const getMediaTypeFromFileExtension = function (fileExtension: string): MediaTypeEnum {
    if (isIncludedInListIgnoringCase(IMAGE_FILE_EXTENSIONS, fileExtension)) {
        return MediaTypeEnum.Image;
    } else if (isIncludedInListIgnoringCase(VIDEO_FILE_EXTENSIONS, fileExtension)) {
        return MediaTypeEnum.Video;
    } else {
        return MediaTypeEnum.Attachment;
    }
};

export const getUploadTypeFromMediaType = function (mediaType: MediaTypeEnum): FileUploadType {
    switch (mediaType) {
        case MediaTypeEnum.Attachment:
            return FileUploadType.Attachment;
        case MediaTypeEnum.Image:
            return FileUploadType.Image;
        case MediaTypeEnum.Video:
            return FileUploadType.Video;
        default:
            throw new Error("Cannot get FileUploadType. Given MediaTypeEnum not supported.");
    }
};

const addPostProcessingParts = function (fileModel: FileModel, postProcessingOptions: FileUploadPostProcessingOptions) {
    switch (postProcessingOptions) {
        case FileUploadPostProcessingOptions.WholeSlideImage:
            fileModel.wholeSlideImageFile = new WholeSlideImageFileModel({
                status: WholeSlideImageFileStatusEnum.Uploading,
            });
            break;
        case FileUploadPostProcessingOptions.Video:
            fileModel.videoFile = new VideoFileModel({
                status: VideoFileStatusEnum.Uploading,
            });
            break;
    }
};
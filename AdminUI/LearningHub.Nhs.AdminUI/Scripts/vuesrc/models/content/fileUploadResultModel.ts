import { ResourceType } from "../../constants";

export class FileUploadResult {
    fileId: number;
    fileName: string;
    fileLocation: string;
    invalid: boolean;
    fileTypeId: number;
    fileSizeKb: number;
    attachedFileTypeId: number;
}
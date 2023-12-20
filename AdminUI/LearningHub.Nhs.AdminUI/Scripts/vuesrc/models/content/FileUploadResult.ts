import { ResourceType } from "../../constants";

export class FileUploadResult {
    fileId: number;
    pageSectionDetailId: number;
    fileName: string;
    fileLocation: string;
    invalid: boolean;
    fileTypeId: number;
    resourceType: ResourceType;
    fileSizeKb: number;
    attachedFileTypeId: number;
}
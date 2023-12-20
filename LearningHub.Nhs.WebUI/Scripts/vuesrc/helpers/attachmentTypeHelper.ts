import { AttachmentTypeEnum } from "../models/contribute-resource/files/attachmentTypeEnum";
import { isIncludedInListIgnoringCase } from "./utils";

const MS_WORD_FILE_TYPES = [".doc", ".docm", ".docx", ".dot", ".dotm", ".dotx"];
const DOCUMENT_FILE_TYPES = [".odt", ".pages"];
const MS_EXCEL_FILE_TYPES = [".xla", ".xlam", ".xls", ".xlsb", ".xlsm", ".xlsx",
    ".xlt", ".xltm", ".xltx", ".xlw"];
const SPREADSHEET_FILE_TYPES = [".csv", ".dat", ".dta", ".numbers", ".ods",
    ".sav", ".sxc", ".tsv", ".zsav"];
const MS_POWERPOINT_FILE_TYPES = [".pot", ".potm", ".potx", ".ppa", ".ppam", ".pps", ".ppsm",
    ".ppsx", ".ppt", ".pptm", ".pptx"];
const PRESENTATION_FILE_TYPES = [".key", ".odp"];
const PDF_FILE_TYPES = [".pdf"];
const ZIP_FILE_TYPES = [".7z", ".gz", ".tar", ".zip"];
const IMAGE_CAROUSEL_FILE_TYPES =[".png", ".jpeg", ".jpg"];
const VIEWABLE_IMAGE_FILE_TYPES = [".apng", ".bmp", ".cur", ".gif", ".jfif", ".jpeg", ".jpg", ".pjp", ".pjpeg", ".png", ".svg", ".webp"];

export const getAttachmentTypeByExtension = function (fileExtension: string): AttachmentTypeEnum {
    if (isIncludedInListIgnoringCase(MS_WORD_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.MsWord;
    } else if (isIncludedInListIgnoringCase(DOCUMENT_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.Document;
    } else if (isIncludedInListIgnoringCase(MS_EXCEL_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.MsExcel;
    } else if (isIncludedInListIgnoringCase(SPREADSHEET_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.SpreadSheet;
    } else if (isIncludedInListIgnoringCase(MS_POWERPOINT_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.MsPowerPoint;
    } else if (isIncludedInListIgnoringCase(PRESENTATION_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.Presentation;
    } else if (isIncludedInListIgnoringCase(PDF_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.Pdf;
    } else if (isIncludedInListIgnoringCase(ZIP_FILE_TYPES, fileExtension)) {
        return AttachmentTypeEnum.Zip;
    } else {
        return AttachmentTypeEnum.Other;
    }
}

export const getIconUrl = function (attachmentTypeEnum: AttachmentTypeEnum): string {
    switch (attachmentTypeEnum){
        case AttachmentTypeEnum.MsWord:
            return "/images/fileicons/a-mword-icon.svg";
        case AttachmentTypeEnum.Document:
            return "/images/fileicons/a-document-icon.svg";
        case AttachmentTypeEnum.MsExcel:
            return "/images/fileicons/a-mexcel-icon.svg";
        case AttachmentTypeEnum.SpreadSheet:
            return "/images/fileicons/a-spreadsheet-icon.svg";
        case AttachmentTypeEnum.MsPowerPoint:
            return "/images/fileicons/a-mppoint-icon.svg";
        case AttachmentTypeEnum.Presentation:
            return "/images/fileicons/a-presentation-icon.svg";
        case AttachmentTypeEnum.Pdf:
            return "/images/fileicons/a-pdf-icon.svg";
        case AttachmentTypeEnum.Zip:
            return "/images/fileicons/a-zip-icon.svg";
        case AttachmentTypeEnum.Other:
            return "/images/fileicons/a-default-icon.svg";
        default:
            throw new Error("Could not get icon URL. Attachment type not supported.");
    }
}

export const getImageIconUrl = function (): string {
    return "/images/fileicons/jpeg.svg";
}

export const getImageAltText = function (): string {
    return "Image Icon";
}

export const isImageFileViewable = function (fileName : string): boolean {
    let isViewable = false;
    
    VIEWABLE_IMAGE_FILE_TYPES.forEach((element) => {
        if (fileName.toLowerCase().endsWith(element)) {
            isViewable = true;
        }
    });
    
    return isViewable;
}

export const isImageValidForCarousel = function (fileName : string): boolean {
    let isValid = (fileType: string) => fileName.toLowerCase().endsWith(fileType);
    return IMAGE_CAROUSEL_FILE_TYPES.some(isValid)
}

export const getAltText = function (attachmentTypeEnum: AttachmentTypeEnum): string {
    switch (attachmentTypeEnum){
        case AttachmentTypeEnum.MsWord:
            return "MS Word icon";
        case AttachmentTypeEnum.Document:
            return "Document icon";
        case AttachmentTypeEnum.MsExcel:
            return "MS Excel icon";
        case AttachmentTypeEnum.SpreadSheet:
            return "Spreadsheet icon";
        case AttachmentTypeEnum.MsPowerPoint:
            return "MS PowerPoint icon";
        case AttachmentTypeEnum.Presentation:
            return "Presentation icon";
        case AttachmentTypeEnum.Pdf:
            return "PDF icon";
        case AttachmentTypeEnum.Zip:
            return "ZIP icon";
        case AttachmentTypeEnum.Other:
            return "File icon";
        default:
            throw new Error("Could not get alt text. Attachment type not supported.");
    }
}
import { ResourceType, VersionStatus, ResourceAccessibility } from "../../constants"
import { AuthorModel } from "./authorModel";
import { KeywordModel } from "./keywordModel";
import { FlagModel } from "../flagModel";

export class ContributeResourceDetailModel {
    resourceVersionId: number = 0;
    resourceId: number = 0;
    resourceType: ResourceType = ResourceType.UNDEFINED;
    currentResourceVersionId: number = null;
    currentResourceVersionStatusEnum: VersionStatus = null;
    resourceAccessibilityEnum: ResourceAccessibility = ResourceAccessibility.None;
    title: string = '';
    additionalInformation: string = '';
    description: string = '';
    resourceLicenceId: number = 0;
    publishedResourceCatalogueId: number = 0;
    resourceCatalogueId: number = 0;
    nodeId: number = 0;
    primaryCatalogueNodeId: number = 1;
    sensitiveContent: boolean = false;
    resourceAuthors: AuthorModel[] = [] as AuthorModel[];
    resourceKeywords: KeywordModel[] = [] as KeywordModel[];
    flags: FlagModel[] = [] as FlagModel[];
    certificateEnabled: boolean = null;
    resourceProviderId: number = null;
}
export class ResourceFileModel {
    resourceVersionId: number = 0;
    fileId: number = 0;
    fileName: string = '';
    fileTypeId: number = 0;
    fileSizeKb: number = 0;
    fileLocation: string = '';
}
export class AttachedFileModel {
    fileId: number = 0;
    fileName: string = '';
    fileTypeId: number = 0;
    fileSizeKb: number = 0;
}
export class GenericFileResourceModel {
    resourceVersionId: number = 0;
    file: ResourceFileModel = new ResourceFileModel();
    authoredYear: number = 0;
    authoredMonth: number = 0;
    authoredDayOfMonth: number = 0;
    esrLinkType: number = 1;
}
export class ScormResourceModel {
    id: number = 0;
    resourceVersionId: number = 0;
    useDefaultPopupWindowSize: boolean = true;
    popupHeight: number = 768;
    popupWidth: number = 1024;
    esrLinkType: number = 1;
    canDownload: boolean = false;
    clearSuspendData: boolean = false;
    file: ResourceFileModel = new ResourceFileModel();
}

export class ExternalReferenceUserAgreement {
    externalReferenceId: number;
}
export class ImageResourceModel {
    resourceVersionId: number = 0;
    file: ResourceFileModel = new ResourceFileModel();
    altTag: string = '';
}
export class VideoResourceModel {
    resourceVersionId: number = 0;
    file: ResourceFileModel = new ResourceFileModel();
    transcriptFile: ResourceFileModel = null;
    closedCaptionsFile: ResourceFileModel = null;
}
export class AudioResourceModel {
    resourceVersionId: number = 0;
    file: ResourceFileModel = new ResourceFileModel();
    transcriptFile: ResourceFileModel = null;
}
export class ArticleResourceModel {
    resourceVersionId: number = 0;
    description: string = '';
    files: AttachedFileModel[] = [] as AttachedFileModel[];
}
export class WeblinkResourceModel {
    resourceVersionId: number = 0;
    url: string = '';
    displayText: string = '';
}
export class HtmlResourceModel {
    resourceVersionId: number = 0;
    useDefaultPopupWindowSize: boolean = true;
    popupHeight: number = 768;
    popupWidth: number = 1024;
    file: ResourceFileModel = new ResourceFileModel();
    esrLinkType: number = 1;
}

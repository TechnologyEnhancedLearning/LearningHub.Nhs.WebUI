export enum CardResourceType {
    DOWNLOAD = 0,
    VIDEO = 1,
    LINK = 2,
    TEXT = 3
};

export enum UploadResourceType {
    NONE = 0,
    FILEUPLOAD = 1,
    ARTICLE = 2,
    WEBLINK = 3,
    EMBEDRESOURCE = 4,
    EQUIPMENT = 5,
    SCORM = 6,
    HTML = 12
};

export enum ResourceType {
    UNDEFINED = 0,
    ARTICLE = 1,
    AUDIO = 2,
    EMBEDDED = 3,
    EQUIPMENT = 4,
    IMAGE = 5,
    SCORM = 6,
    VIDEO = 7,
    WEBLINK = 8,
    GENERICFILE = 9,
    CASE = 10,
    ASSESSMENT = 11,
    HTML = 12,
};

//export enum ResourceFileType {
//    RESOURCE = 1,
//    TRANSCRIPT = 2,
//    CAPTION = 3
//};

export enum VersionStatus {
    DRAFT = 1,
    PUBLISHED = 2,
    UNPUBLISHED = 3,
    PUBLISHING = 4,
    SUBMITTED = 5,
    FAILED = 6
};

export enum ResourceAccessibility {    
    PublicAccess = 1,
    GeneralAccess = 2,
    FullAccess = 3,
    None = 4,
};

export enum ActivityStatus {
    Launched = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4,
    Passed = 5,
    Downloaded = 6,
    Incomplete = 7
};

export enum AccessRequestStatus {
    Pending = 0,
    Approved = 1,
    Denied = 2
};

export enum MediaResourceActivityType {
    Play = 1,
    Pause = 2,
    End = 3,
    Playing = 4
};

export const AzureMediaPlayerOptions = {
    "nativeControlsForTouch": false,
    controls: true,
    autoplay: false,
    width: "100%",
    height: "100%"
};

export enum RoleEnum {
    Editor = 1,
    Reader = 2,
    LocalAdmin = 3
};

export enum ScopeTypeEnum {
    Catalogue = 1
};

export enum NodeType {
    Resource = 0,
    Catalogue = 1,
    Course = 2,
    Folder = 3,
    ExternalOrganisation = 4
};
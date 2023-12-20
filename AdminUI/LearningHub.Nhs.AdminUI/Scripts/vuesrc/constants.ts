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
}

export enum NodeType {
    Catalogue = 1,
    Course = 2,
    Folder = 3, 
    ExternalOrganisation = 4
}

export enum VersionStatus {
    DRAFT = 1,
    PUBLISHED = 2,
    UNPUBLISHED = 3,
    PUBLISHING = 4,
    SUBMITTED = 5,
    FAILED = 6
}
import { ArticleModel } from './articleModel';
import { AudioModel } from './audioModel';
import { EmbedCodeModel } from './embedCodeModel';
import { EquipmentModel } from './equipmentModel';
import { ImageModel } from './imageModel';
import { GenericFileModel } from './genericFileModel';
import { WebLinkModel } from './webLinkModel';
import { VideoModel } from './videoModel';
import { ResourceType } from '../constants';
import { VersionStatus } from '../constants';
import { CatalogueModel } from './catalogueModel';
import { CaseResourceModel } from './contribute-resource/caseResourceModel';
import { ScormFileModel,ScormContentDetailsModel } from './scormModel';
import { AssessmentModel } from "./contribute-resource/assessmentModel";

export class ResourceItemModel {

    id: number;
    resourceId: number;
    resourceVersionId: number;
    nodePathId: number;
    resourceTypeEnum: ResourceType;
    majorVersion: number;
    minorVersion: number;
    title: string;
    description: string;
    additionalInformation: string;    
    publishedBy: string;
    publishedDate: Date;
    firstPublishedDate: Date;
    versionStatusEnum: VersionStatus; 
    versionDescription: string;
    sensitiveContent: boolean;
    authors: string[];
    authoredDate: Date;
    articleDetails: ArticleModel;
    audioDetails: AudioModel;
    videoDetails: VideoModel;
    embedCodeDetails: EmbedCodeModel;
    equipmentDetails: EquipmentModel;
    imageDetails: ImageModel;
    genericFileDetails: GenericFileModel;
    scormDetails: ScormFileModel;
    webLinkDetails: WebLinkModel;  
    caseDetails: CaseResourceModel;
    assessmentDetails: AssessmentModel;
    licenseName: string;
    licenseUrl: string;
    displayForContributor: boolean;
    resourceVersionIdInEdit: number;
    resourceVersionIdPublishing: number;
    unpublishedByAdmin: boolean;
    readOnly: boolean;
    catalogue: CatalogueModel;
    bookmarkId: number;
    isBookmarked: boolean;
    constructor(init?: Partial<ResourceItemModel>) {
        Object.assign(this, init);

        if(init?.caseDetails) {
            this.caseDetails = new CaseResourceModel(init.caseDetails);
        }

        if(init?.assessmentDetails) {
            this.assessmentDetails = new AssessmentModel(init.assessmentDetails);
        }
    }
}
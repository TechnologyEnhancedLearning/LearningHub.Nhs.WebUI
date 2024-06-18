import AxiosWrapper from '../axiosWrapper';
import { ResourceHeaderModel } from '../models/resourceHeaderModel';
import { ResourceInformationModel } from '../models/resourceInformationModel';
import { ResourceItemModel } from '../models/resourceItemModel';
import { ResourceVersionHistoryModel } from '../models/resourceVersionHistoryModel';
import { FileTypeModel } from '../models/contribute/fileTypeModel';
import { ResourceLocationsModel } from '../models/resourceLocationsModel';
import { ContributeResourceDetailModel, GenericFileResourceModel, ScormResourceModel, ExternalReferenceUserAgreement, ImageResourceModel, VideoResourceModel, ArticleResourceModel, AudioResourceModel, WeblinkResourceModel, HtmlResourceModel } from '../models/contribute/contributeResourceModel';
import { ResourceType, VersionStatus } from '../constants';
import { LicenceModel } from '../models/contribute/licenceModel';
import { AuthorModel } from '../models/contribute/authorModel';
import { KeywordModel } from '../models/contribute/keywordModel';
import { ContributeSettingsModel } from '../models/contribute/contributeSettingsModel';
import { CatalogueBasicModel } from '../models/catalogueModel';
import { CaseResourceModel } from '../models/contribute-resource/caseResourceModel';
import { FileModel } from '../models/contribute-resource/files/fileModel';
import { ContributeConfiguration } from "../models/contribute-resource/contributeConfiguration";
import { CatalogueModel } from '../models/catalogueModel';
import { RoleUserGroupModel } from '../models/roleUserGroupModel';
import { ScormContentDetailsModel } from '../models/scormModel';
import { LearningHubValidationResultModel } from '../models/learningHubValidationResultModel';
import { AssessmentModel } from '../models/contribute-resource/assessmentModel';
import { AssessmentTypeEnum } from "../models/contribute-resource/blocks/assessments/assessmentTypeEnum";
import { MyContributionsCardModel } from "../models/contribute/mycontributionsCardModel";
import { MyContributionsBasicDetailsModel } from "../models/contribute/myContributionsBasicDetailsModel";

const getContributeConfiguration = async function (): Promise<ContributeConfiguration> {
    return await AxiosWrapper.axios.get<ContributeConfiguration>('/api/Contribute/GetConfiguration')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getContributeConfiguration:' + e);
            throw e;
        });
};

const getContributeSettings = async function (): Promise<ContributeSettingsModel> {
    return await AxiosWrapper.axios.get<ContributeSettingsModel>('/api/Contribute/GetSettings')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getContributeSettings:' + e);
            throw e;
        });
};

const getHeader = async function (id: number): Promise<ResourceHeaderModel> {
    return await AxiosWrapper.axios.get<ResourceHeaderModel>('/api/Resource/GetHeaderById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getResource:' + e);
            throw e;
        });
};

const getItem = async function (id: number): Promise<ResourceItemModel> {
    return await AxiosWrapper.axios.get<ResourceItemModel>('/api/Resource/GetItemById/' + id)
        .then(response => {
            if (!response || !response.data) {

            }
            if (response.data.versionStatusEnum === 3 && !response.data.displayForContributor) {
                window.location.pathname = './Resource/unpublished';
                return null;
            } else {
                return new ResourceItemModel(response.data);
            }
        })
        .catch(e => {
            console.log('getItem:' + e);
            throw e;
        });
};


const userHasResourceCertificate = async function (id: number): Promise<boolean> {

    return await AxiosWrapper.axios.get<boolean>('/api/MyLearning/CheckCertificateAvailability/'+ id)
        .then(response => {
            if (response.data == true) { return true } else { return false; }
        })
        .catch(e => {
            console.log('userHasResourceCertificate:' + e);
            throw e;
        });
};

const getUploadResourceTypes = async function (): Promise<{ id: number, description: string }[]> {
    return [
        { id: 6, description: 'elearning package (SCORM 1.2)' },
        { id: 2, description: 'Article' },
        { id: 3, description: 'Web link' },
        { id: 1, description: 'File upload' },
        { id: 12, description: 'HTML' },
        //{ id: 4, description: 'Video or audio embed code' },
        //{ id: 5, description: 'Equipment or facilities' }
    ];
};

const getFileTypes = async function (): Promise<FileTypeModel[]> {
    return await AxiosWrapper.axios.get<FileTypeModel[]>('/api/Contribute/GetFileType')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getFileTypes:' + e);
            throw e;
        });
};

const getFileTypesExtensions = (item: FileTypeModel) => { return item.extension };

const getLicences = async function (): Promise<LicenceModel[]> {
    return await AxiosWrapper.axios.get<LicenceModel[]>('/api/Contribute/GetLicences')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getLicences:' + e);
            throw e;
        });
};

const getCataloguesForUser = async function (): Promise<CatalogueBasicModel[]> {
    return await AxiosWrapper.axios.get<CatalogueBasicModel[]>('/api/Contribute/GetCataloguesForUser')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getCataloguesForUser:' + e);
            throw e;
        });
};

const getResourceVersion = async function (id: number): Promise<ContributeResourceDetailModel> {
    return await AxiosWrapper.axios.get<ContributeResourceDetailModel>('/api/Contribute/GetResourceVersionById/' + id)
        .then(response => {
            if (response.status === 204) {
                window.location.pathname = './Contribute/no-longer-available';
                return null;
            } else {
                return response.data;
            }
        })
        .catch(error => {
            if (error.response && error.response.status === 404) {
                window.location.pathname = './Home/AccessDenied';
            }
            console.log('getResourceVersion:' + error);
            throw error;
        });
};

const getGenericFileDetail = async function (id: number): Promise<GenericFileResourceModel> {
    return await AxiosWrapper.axios.get<GenericFileResourceModel>('/api/Resource/GetGenericFileDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getGenericFileDetail:' + e);
            throw e;
        });
};

const getHtmlDetail = async function (id: number): Promise<HtmlResourceModel> {
    return await AxiosWrapper.axios.get<HtmlResourceModel>('/api/Resource/GetHtmlDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getHtmlDetail:' + e);
            throw e;
        });
};

const getScormDetail = async function (id: number): Promise<ScormResourceModel> {
    return await AxiosWrapper.axios.get<ScormResourceModel>('/api/Resource/GetScormDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getScormDetail:' + e);
            throw e;
        });
};

const getScormContentDetails = async function (id: number): Promise<ScormContentDetailsModel> {
    return await AxiosWrapper.axios.get<ScormContentDetailsModel>('/api/Resource/GetExternalContentDetails/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getScormContentDetails:' + e);
            throw e;
        });
};

const recordExternalReferenceUserAgreement = async function (externalReferenceId: number) {

    const url = `/api/resource/recordexternalreferenceuseragreement`;

    const params = {
        externalReferenceId: externalReferenceId,
    }
    return await AxiosWrapper.axios.post<boolean>(url, params)
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('recordExternalReferenceUserAgreement:' + e);
            throw e;
        });
};
const getImageDetail = async function (id: number): Promise<ImageResourceModel> {
    return await AxiosWrapper.axios.get<ImageResourceModel>('/api/Resource/GetImageDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getImageDetail:' + e);
            throw e;
        });
};

const getVideoDetail = async function (id: number): Promise<VideoResourceModel> {
    return await AxiosWrapper.axios.get<VideoResourceModel>('/api/Resource/GetVideoDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getVideoDetail:' + e);
            throw e;
        });
};

const getVideoFileAuthToken = async function (assetId: string): Promise<string> {
    return await AxiosWrapper.axios.get<string>('/api/Resource/GetVideoFileContentAuthenticationToken/' + assetId)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getVideoFileAuthToken:' + e);
            throw e;
        });
};

const getAudioDetail = async function (id: number): Promise<AudioResourceModel> {
    return await AxiosWrapper.axios.get<AudioResourceModel>('/api/Resource/GetAudioDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getAudioDetail:' + e);
            throw e;
        });
};

const getArticleDetail = async function (id: number): Promise<ArticleResourceModel> {
    return await AxiosWrapper.axios.get<ArticleResourceModel>('/api/Resource/GetArticleDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getArticleDetail:' + e);
            throw e;
        });
};

const getWeblinkDetail = async function (id: number): Promise<WeblinkResourceModel> {
    return await AxiosWrapper.axios.get<WeblinkResourceModel>('/api/Resource/GetWeblinkDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getWeblinkDetail:' + e);
            throw e;
        });
};

const getCaseDetail = async function (id: number): Promise<CaseResourceModel> {
    return await AxiosWrapper.axios.get<CaseResourceModel>('/api/Resource/GetCaseDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getCaseDetail:' + e);
            throw e;
        });
};

const getAssessmentDetail = async function (id: number): Promise<AssessmentModel> {
    return await AxiosWrapper.axios.get<AssessmentModel>('/api/Resource/GetAssessmentDetailsById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getAssessmentDetail:' + e);
            throw e;
        });
};

const getFileStatusDetails = async function (fileIds: number[]): Promise<FileModel[]> {
    const queryString = fileIds.map(fileId => `fileIds=${fileId}`).join('&');
    return await AxiosWrapper.axios.get<FileModel[]>('/api/Resource/GetFileStatusDetails/?' + queryString)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getCaseDetail:' + e);
            throw e;
        });
};

const acceptSensitiveContent = async function (resourceVersionId: number): Promise<boolean> {
    return await AxiosWrapper.axios.post<boolean>('/api/Resource/AcceptSensitiveContent/' + resourceVersionId.toString())
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('acceptSensitiveContent:' + e);
            throw e;
        });
};

const deleteArticleFile = async function (resourceVersionId: number, fileId: number): Promise<boolean> {
    const params = {
        resourceVersionId: resourceVersionId,
        fileId: fileId
    }
    return await AxiosWrapper.axios.post<boolean>('/api/Contribute/DeleteArticleFile', params)
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('deleteArticleFile:' + e);
            throw e;
        });
};

const deleteResourceAttributeFile = async function (resourceType: ResourceType, resourceVersionId: number, fileTypeId: number): Promise<boolean> {
    const params = {
        resourceType: resourceType,
        resourceVersionId: resourceVersionId,
        attachedFileType: fileTypeId
    }
    return await AxiosWrapper.axios.post<boolean>('/api/Contribute/DeleteResourceAttributeFile', params)
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('deleteResourceAttributeFile:' + e);
            throw e;
        });
};

const deleteResourceVersion = async function (resourceVersionId: number): Promise<LearningHubValidationResultModel> {
    return await AxiosWrapper.axios.delete<LearningHubValidationResultModel>('/api/Contribute/DeleteResourceVersion/' + resourceVersionId.toString())
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('deleteResourceVersion:' + e);
            throw e;
        });
};

const addAuthor = async function (resourceVersionId: number, author: AuthorModel): Promise<AuthorModel> {
    return await AxiosWrapper.axios.post<AuthorModel>('/api/Contribute/AddResourceAuthor', author)
        .then(response => {
            //return response.data;
            const auth = new AuthorModel();
            auth.id = response.data.id;
            auth.resourceVersionId = response.data.resourceVersionId;
            auth.authorName = response.data.authorName;
            auth.organisation = response.data.organisation;
            auth.role = response.data.role;
            auth.isContributor = response.data.isContributor;
            return auth;
        })
        .catch(e => {
            console.log('addAuthor:' + e);
            throw e;
        });
};

const deleteAuthor = async function (resourceVersionId: number, authorId: number): Promise<boolean> {
    const params = {
        resourceVersionId: resourceVersionId,
        authorId: authorId
    }
    return await AxiosWrapper.axios.post<boolean>('/api/Contribute/DeleteResourceAuthor', params)
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('deleteAuthor:' + e);
            throw e;
        });
};

const addKeyword = async function (resourceVersionId: number, keyword: KeywordModel): Promise<KeywordModel> {
    return await AxiosWrapper.axios.post<KeywordModel>('/api/Contribute/AddResourceKeyword', keyword)
        .then(response => {
            const keyword = new KeywordModel();
            keyword.id = response.data.id;
            keyword.resourceVersionId = response.data.resourceVersionId;
            keyword.keyword = response.data.keyword;
            return keyword;
        })
        .catch(e => {
            console.log('addKeyword:' + e);
            throw e;
        });
};

const deleteKeyword = async function (resourceVersionId: number, keywordId: number): Promise<boolean> {
    const params = {
        resourceVersionId: resourceVersionId,
        keywordId: keywordId
    }
    return await AxiosWrapper.axios.post<boolean>('/api/Contribute/DeleteResourceKeyword', params)
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('deleteKeyword:' + e);
            throw e;
        });
};

const publishResource = async function (resourceVersionId: number, notes: string): Promise<boolean> {
    const params = {
        resourceVersionId: resourceVersionId,
        notes: notes
    }
    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/Contribute/PublishResourceVersion', params)
        .then(response => {
            return response.data.isValid;
        })
        .catch(e => {
            if (e.message.indexOf("Network") > -1) {
                window.location.pathname = './Home';
                return false;
            }
            console.log('deleteArticleFile:' + e);
            throw e;
        });
};

const duplicateResource = async function (resourceVersionId: number, resourceCatalogueId: number): Promise<number> {
    const params = {
        resourceVersionId,
        resourceCatalogueId,
    };

    return await AxiosWrapper.axios.post('/api/Resource/DuplicateResource', params)
        .then(response => {
            return response.data.createdId;
        })
        .catch(e => {
            console.log('contributeApi.duplicateResource:' + e);
            throw e;
        });
};

const duplicateBlocks = async function (sourceResourceId: number, blockIds: number[], assessmentType?: AssessmentTypeEnum, destinationResourceId?: number): Promise<number> {
    const params = {
        sourceResourceId,
        blockIds,
        assessmentType,
        destinationResourceId
    };
    
    return await AxiosWrapper.axios.post('/api/Resource/DuplicateBlocks', params)
        .then(response => {
            return response.data.createdId;
        })
        .catch(e => {
            console.log('contributeApi.duplicateBlocks:' + e);
            throw e;
        });
};

const getMyContributions = async function (resourceType: ResourceType, status: VersionStatus, catalogueId?: number): Promise<MyContributionsBasicDetailsModel[]> {
    const params = {
        status,
        resourceType,
        catalogueId
    };
    
    return await AxiosWrapper.axios.post('/api/Resource/MyContributions', params)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('contributeApi.getMyContributions:' + e);
            throw e;
        });
};

const getContributeAVResourceFlag = async function (): Promise<boolean> {
    return await AxiosWrapper.axios.get<boolean>('/Resource/GetContributeAVResourceFlag')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('GetContributeAVResourceFlag:' + e);
            throw e;
        });
};

const getDisplayAVResourceFlag = async function (): Promise<boolean> {
    return await AxiosWrapper.axios.get<boolean>('/Resource/GetDisplayAVResourceFlag')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('GetDisplayAVResourceFlag:' + e);
            throw e;
        });
};

const getAVUnavailableView = async function (): Promise<string> {
    return await AxiosWrapper.axios.get('/Resource/GetAVUnavailableView')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.error('Error fetching shared partial view:', e)
            throw e;
        });
};
const getMKPlayerKey = async function (): Promise<string> {
    return await AxiosWrapper.axios.get('/Resource/GetMKPlayerKey')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.error('Error fetching Media Kind MKPlayer Key', e)
            throw e;
        });
};

export const resourceData = {
    getContributeConfiguration,
    getContributeSettings,
    getHeader,
    getItem,
    userHasResourceCertificate,
    getUploadResourceTypes,
    getFileTypes,
    getLicences,
    getCataloguesForUser,
    getFileTypesExtensions,
    getResourceVersion,
    getGenericFileDetail,
    getHtmlDetail,
    getScormDetail,
    getScormContentDetails,
    recordExternalReferenceUserAgreement,
    getImageDetail,
    getVideoDetail,
    getVideoFileAuthToken,
    getAudioDetail,
    getArticleDetail,
    getWeblinkDetail,
    getCaseDetail,
    getFileStatusDetails,
    acceptSensitiveContent,
    deleteResourceAttributeFile,
    deleteArticleFile,
    deleteResourceVersion,
    addAuthor,
    deleteAuthor,
    addKeyword,
    deleteKeyword,
    publishResource,
    duplicateResource,
    getAssessmentDetail,
    duplicateBlocks,
    getMyContributions,
    getContributeAVResourceFlag,
    getDisplayAVResourceFlag,
    getAVUnavailableView,
    getMKPlayerKey
};
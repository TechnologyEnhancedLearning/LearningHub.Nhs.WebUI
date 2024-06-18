import Vue from 'vue';
import AxiosWrapper from '../axiosWrapper';
import * as _ from "lodash";
import Vuex, { ActionContext, Store } from 'vuex';
import { GetterTree, MutationTree, ActionTree } from "vuex";
import { ResourceAccessibility, ResourceType } from '../constants';    
import { debounce } from 'ts-debounce';
import { ContributeResourceDetailModel, ResourceFileModel, GenericFileResourceModel, ScormResourceModel, ImageResourceModel, VideoResourceModel, ArticleResourceModel, AttachedFileModel, AudioResourceModel, WeblinkResourceModel, HtmlResourceModel } from '../models/contribute/contributeResourceModel';
import { FileTypeModel } from '../models/contribute/fileTypeModel';
import { ContributeSettingsModel } from '../models/contribute/contributeSettingsModel';
import { LicenceModel } from '../models/contribute/licenceModel';
import { CatalogueBasicModel } from '../models/catalogueModel';
import { resourceData } from '../data/resource';
import { AuthorModel } from '../models/contribute/authorModel';
import { KeywordModel } from '../models/contribute/keywordModel';
import { contentStructureData } from '../data/contentStructure';
import { HierarchyEditStatusEnum, HierarchyEditModel } from '../models/content-structure/hierarchyEditModel';

import { providerData } from '../data/provider';
import { ProviderModel } from '../models/providerModel';

Vue.use(Vuex); 

export class State {
    saveStatus: string = '';
    isSaving: boolean = false;
    publishAfterSave: boolean = false;
    closeAfterSave: boolean = false;
    saveError: boolean = false;
    currentUserName: string = '';
    resourceLicenseUrl: string = '';
    resourceCertificateUrl: string = '';
    supportUrlExcludedFiles: string = '';
    resourceHeaderLoading: boolean = false;
    resourceLoading: boolean = false;
    commonContentValid: boolean = false;
    commonContentDirty: boolean = false;
    specificContentValid: boolean = false;
    specificContentDirty: boolean = false;
    contributeSettings = new ContributeSettingsModel();
    resourceDetail = new ContributeResourceDetailModel();
    genericFileDetail = new GenericFileResourceModel();
    scormDetail = new ScormResourceModel();
    imageDetail = new ImageResourceModel();
    videoDetail = new VideoResourceModel();
    audioDetail = new AudioResourceModel();
    articleDetail = new ArticleResourceModel();
    weblinkDetail = new WeblinkResourceModel();
    htmlDetail = new HtmlResourceModel();

    fileTypes: FileTypeModel[] = null;
    licences: LicenceModel[] = null;
    userCatalogues: CatalogueBasicModel[] = null;
    fileUpdated: Date = null;
    counterInterval: any = undefined;
    hierarchyEdit: HierarchyEditModel = null;
    hierarchyEditLoaded: boolean = false;
    userProviders: ProviderModel[] = null;
    contributeAVResourceFlag: boolean;
    learnAVResourceFlag: boolean;
    getAVUnavailableView: string = '';
    getMKPlayerLicenceKey: string = '';

    get previousVersionExists(): boolean {
        if (this.resourceDetail.currentResourceVersionId) {
            return true;
        } else {
            return false;
        }
    }
    get mainTitle(): string {
        if (!this.resourceDetail || this.resourceHeaderLoading) {
            return '';
        } else if (this.previousVersionExists) {
            return 'Edit resource';
        } else {
            return 'Contribute a resource';
        }
    }
    loadHierarchyEdit(): void{
        // TODO: resourceCatalogueId needs to be replaced by the root node path id
        contentStructureData.getHierarchyEdit(this.resourceDetail.resourceCatalogueId).then(response => {
            this.hierarchyEdit = response[0];
            this.hierarchyEditLoaded = true;
            if (this.hierarchyEdit != null && this.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft) {
                clearInterval(this.counterInterval);;
            }
        });
    }
}
const state = new State();

class ApiRequest {
    data: string;
    action: string;
}

const api = {
    save: debounce(function (request: ApiRequest, callback: Function) {
        AxiosWrapper.axios.post('/api/Contribute/' + request.action, request.data)
            .then(response => {
                callback(response.data);
            })
            .catch(e => {
                console.log('processContributeSave:' + e);
                callback(0, true);
            });

    }, 500, { isImmediate: false })
}

const setCommonContentState = function (state: State) {    
    state.commonContentValid = state.resourceDetail.resourceVersionId > 0
        && state.resourceDetail.resourceType !== ResourceType.UNDEFINED        
        && state.resourceDetail.resourceAccessibilityEnum !== ResourceAccessibility.None
        && state.resourceDetail.description !== ''
        && (state.resourceDetail.resourceLicenceId > 0 || state.resourceDetail.resourceType === ResourceType.WEBLINK)
        && state.resourceDetail.resourceCatalogueId > 0
        && state.resourceDetail.resourceAuthors.length > 0
        && state.resourceDetail.certificateEnabled !== null
        && state.resourceDetail.resourceKeywords.length > 0;
}
const setSpecificContentState = function (state: State) {
    
    switch (state.resourceDetail.resourceType) {
        case ResourceType.GENERICFILE:
            state.specificContentValid = !!state.genericFileDetail
                && !!state.genericFileDetail.file
                && !!state.genericFileDetail.file.fileName
                && !!state.genericFileDetail.file.fileName.length;
            break;
        case ResourceType.HTML:
            state.specificContentValid = !!state.htmlDetail
                && !!state.htmlDetail.file
                && !!state.htmlDetail.file.fileName
                && !!state.htmlDetail.file.fileName.length;
            break;
        case ResourceType.SCORM:
            state.specificContentValid = !!state.scormDetail && !!state.scormDetail.file;
            break;
        case ResourceType.VIDEO:
            state.specificContentValid = !!state.videoDetail && !!state.videoDetail.file;
            break;
        case ResourceType.AUDIO:
            state.specificContentValid = !!state.imageDetail && !!state.imageDetail.file;
            break;
        case ResourceType.IMAGE:
            state.specificContentValid = state.imageDetail.altTag !== '';
            break;
        case ResourceType.ARTICLE:
            state.specificContentValid = state.articleDetail.description !== '';
            break;
        case ResourceType.WEBLINK:
            state.specificContentValid = state.weblinkDetail.url !== '';
            break;
        default:
    }

    setCommonContentState(state);
}

const autosaverPlugin = function (store: Store<State>) {    

    store.subscribe(function (mutation: any, state: State) {        

        let apiAction: string = '';
        let data: any;

        switch (mutation.type) {
            case "saveResourceType":
            case "saveResourceDetail":
                data = state.resourceDetail;
                apiAction = "saveResourceDetail";
                break;
            case "saveGenericFileDetail":
                data = state.genericFileDetail;
                apiAction = "saveGenericFileDetail";
                break;
            case "saveScormDetail":
                data = state.scormDetail;
                apiAction = "saveScormDetail";
                break;
            case "saveImageDetail":
                data = state.imageDetail;
                apiAction = "saveImageDetail";
                break;
            case "saveVideoDetail":
                data = state.videoDetail;
                apiAction = "saveVideoDetail";
                break;
            case "saveAudioDetail":
                data = state.audioDetail;
                apiAction = "saveAudioDetail";
                break;
            case "saveArticleDetail":
                data = state.articleDetail;
                apiAction = "saveArticleDetail";
                break;
            case "saveWeblinkDetail":
                data = state.weblinkDetail;
                apiAction = "saveWeblinkDetail";
                break;
            case "saveHtmlDetail":
                data = state.htmlDetail;
                apiAction = "saveHtmlDetail";
                break;
            default:
                break;
        }

        if (apiAction !== '') {

            data.resourceVersionId = state.resourceDetail.resourceVersionId;
            const request: ApiRequest = { 
                data: data,
                action: apiAction
            };

            store.commit('setSaveStatus', 'Automatically saving as draft...');

            api.save(request, function (resourceVersionId: number, failed: boolean = false) {
                if (failed) {
                    store.commit('setSaveStatus', 'Error save failed' );
                } else {
                    store.commit('setResourceVersionId', resourceVersionId)
                    store.commit('setSaveStatus', 'Saved')

                    if (apiAction === "saveResourceDetail") {
                        setCommonContentState(state);
                    } else {
                        setSpecificContentState(state);
                    }
                    if (store.state.publishAfterSave) {
                        store.state.publishAfterSave = false;
                    }
                    if (store.state.closeAfterSave) {
                        store.state.closeAfterSave = false;
                    }

                }
            });
        }
    })
}

const mutations = {
    async populateContributeSettings(state: State) {
        state.contributeSettings = await resourceData.getContributeSettings();
    },
    async populateContributeAVResourceFlag(state: State) {
        state.contributeAVResourceFlag = await resourceData.getContributeAVResourceFlag();
    },
    async populateDisplayAVResourceFlag(state: State) {
        state.learnAVResourceFlag = await resourceData.getDisplayAVResourceFlag();
    },
    async populateAVUnavailableView(state: State) {
        state.getAVUnavailableView = await resourceData.getAVUnavailableView();
    },
    async populateMKPlayerLicenceKey(state: State) {
        state.getMKPlayerLicenceKey = await resourceData.getMKPlayerKey();
    },
    async populateScormDetails(state: State, payload: number) {
        const scormDetail = await resourceData.getScormDetail(payload);
        state.scormDetail.canDownload = scormDetail.canDownload;
        state.scormDetail.esrLinkType = scormDetail.esrLinkType;
        state.scormDetail.useDefaultPopupWindowSize = scormDetail.useDefaultPopupWindowSize;
        state.scormDetail.popupWidth = scormDetail.popupWidth;
        state.scormDetail.popupHeight = scormDetail.popupHeight;
        state.scormDetail.file.fileId = scormDetail.file.fileId;
        state.scormDetail.file.fileLocation = scormDetail.file.fileLocation;
        state.scormDetail.file.fileName = scormDetail.file.fileName;
        state.scormDetail.file.fileSizeKb = scormDetail.file.fileSizeKb;
        state.scormDetail.file.fileTypeId = scormDetail.file.fileTypeId;
        state.scormDetail.file.resourceVersionId = scormDetail.file.resourceVersionId;
        state.scormDetail.id = scormDetail.id;
        state.scormDetail.resourceVersionId = scormDetail.resourceVersionId;
        state.scormDetail.clearSuspendData = scormDetail.clearSuspendData;
    },
    async populateResource(state: State, payload: number) {
        state.resourceLoading = true;
        state.resourceHeaderLoading = true;
        const resourceDetail = await resourceData.getResourceVersion(payload);
                
        state.loadHierarchyEdit();
        state.counterInterval = setInterval(() => state.loadHierarchyEdit(), 10000);

        if (state.hierarchyEdit != null && state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft) {
            state.resourceLoading = false;
            return;
        }

        state.resourceHeaderLoading = false;
        switch (resourceDetail.resourceType) {
            case ResourceType.GENERICFILE:
                const genericFileDetail: any = await resourceData.getGenericFileDetail(payload);
                state.resourceDetail = resourceDetail;
                if (genericFileDetail === '') {
                    break;
                }
                state.genericFileDetail = genericFileDetail as GenericFileResourceModel;
                break;
            case ResourceType.HTML:
                const htmlDetail: any = await resourceData.getHtmlDetail(payload);
                state.resourceDetail = resourceDetail;
                if (htmlDetail) {
                    state.htmlDetail = htmlDetail;
                } else {
                    state.htmlDetail = new HtmlResourceModel();
                }
                break;
            case ResourceType.SCORM:                
                const scormDetail = await resourceData.getScormDetail(payload);               
                state.resourceDetail = resourceDetail;
                if (scormDetail) {
                    state.scormDetail = scormDetail;
                } else {
                    state.scormDetail = new ScormResourceModel();
                }
                break;
            case ResourceType.IMAGE:
                const imageDetail: any = await resourceData.getImageDetail(payload);
                state.resourceDetail = resourceDetail;
                if (imageDetail === '') {
                    break;
                }
                state.imageDetail = imageDetail as ImageResourceModel;
                break;
            case ResourceType.VIDEO:
                const videoDetail: any = await resourceData.getVideoDetail(payload);
                state.resourceDetail = resourceDetail;
                if (videoDetail === '') {
                    break;
                }
                state.videoDetail = videoDetail as VideoResourceModel;
                break;
            case ResourceType.AUDIO:
                const audioDetail: any = await resourceData.getAudioDetail(payload);
                state.resourceDetail = resourceDetail;
                if (audioDetail === '') {
                    break;
                }
                state.audioDetail = audioDetail as AudioResourceModel;
                break;
            case ResourceType.ARTICLE:
                const articleDetail = await resourceData.getArticleDetail(payload);
                state.resourceDetail = resourceDetail;
                state.articleDetail = articleDetail;
                break;
            case ResourceType.WEBLINK:
                const weblinkDetail = await resourceData.getWeblinkDetail(payload);
                state.resourceDetail = resourceDetail;
                state.weblinkDetail = weblinkDetail;
                break;
            default:
                state.resourceDetail = resourceDetail;
                break;
        }
        setCommonContentState(state);
        setSpecificContentState(state);
        state.resourceLoading = false;
    },
    async populateFileTypes(state: State) {
        state.fileTypes = await resourceData.getFileTypes();
    },
    async populateLicences(state: State) {
        state.licences = await resourceData.getLicences();
    },
    async populateUsersCatalogues(state: State) {
        state.userCatalogues = await resourceData.getCataloguesForUser();
        if (state.userCatalogues.length === 1) {
            state.resourceDetail.resourceCatalogueId = state.userCatalogues[0].nodeId;
        }
    },
    setUserProviders(state: State, payload: ProviderModel[]) {
        state.userProviders = payload;               
    },
    setCurrentUserName(state: State, payload: string) {
        state.currentUserName = payload;
    },
    setResourceLicenseUrl(state: State, payload: string) {
        state.resourceLicenseUrl = payload;
    },
    setResourceCertificateUrl(state: State, payload: string) {
        state.resourceCertificateUrl = payload;
    },
    setSupportUrlExcludedFiles(state: State, payload: string) {
        state.supportUrlExcludedFiles = payload;
    },
    setResourceVersionId(state: State, payload: number) {
        state.resourceDetail.resourceVersionId = payload;
    },
    setSaveStatus(state, payload) {
        if (payload === '' || payload === 'Saved') {
            state.isSaving = false;
        } else {
            state.isSaving = true;
        }
        state.saveStatus = payload;
        state.saveError = payload.startsWith('Error');
    },
    publishAfterSave(state) {
        state.publishAfterSave = true;
    },
    closeAfterSave(state) {
        state.closeAfterSave = true;
    },
    setResourceType(state: State, resourceType: ResourceType) {
        state.resourceDetail.resourceType = resourceType;
        setSpecificContentState(state);
    },
    setResourceFile(state: State, file: ResourceFileModel) {
        switch (state.resourceDetail.resourceType) {
            case ResourceType.GENERICFILE:
                state.genericFileDetail.file = file;
                break;
            case ResourceType.SCORM:
                state.scormDetail.file = file;
                break;
            case ResourceType.IMAGE:
                state.imageDetail.file = file;
                break;
            case ResourceType.VIDEO:
                state.videoDetail.file = file;
                break;
            case ResourceType.AUDIO:
                state.audioDetail.file = file;
                break;
            case ResourceType.HTML:
                state.htmlDetail.file = file;
                break;
        }
        state.fileUpdated = new Date();
        setSpecificContentState(state);
    },
    setVideoTranscriptFile(state: State, file: ResourceFileModel) {
        state.videoDetail.transcriptFile = file;
    },
    setVideoclosedCaptionsFile(state: State, file: ResourceFileModel) {
        state.videoDetail.closedCaptionsFile = file;
    },
    setSpecificContentState(state: State, isValid: boolean) {
        state.specificContentValid = isValid;
    },
    setCommonContentDirty(state: State, isDirty: boolean) {
        state.commonContentDirty = isDirty;
    },
    setSpecificContentDirty(state: State, isDirty: boolean) {
        state.specificContentDirty = isDirty;
    },
    removeVideoAttributeFile(state: State, fileTypeId: number) {
        switch (fileTypeId) {
            case 1:
                state.videoDetail.transcriptFile = null;
                break;
            case 2:
                state.videoDetail.closedCaptionsFile = null;
                break;
        }
    },
    removeAudioAttributeFile(state: State, fileTypeId: number) {
        switch (fileTypeId) {
            case 1:
                state.videoDetail.transcriptFile = null;
                break;
        }
    },
    setAudioTranscriptFile(state: State, file: ResourceFileModel) {
        state.audioDetail.transcriptFile = file;
    },
    setInitialCatalogue(state: State, catalogueId: number) {
        state.resourceDetail.resourceCatalogueId = Number(catalogueId);

        // TODO: catalogueId needs to be replaced by rootNodePathId
        contentStructureData.getHierarchyEdit(catalogueId).then(response => {
            state.hierarchyEdit = response[0];
        });

        // If contributing resource into catalogue root, the nodeId is the same. When contributing into a folder, setInitialNode will update this.
        state.resourceDetail.nodeId = Number(catalogueId);
    },
    setInitialNode(state: State, nodeId: number) {
        state.resourceDetail.nodeId = Number(nodeId);
    },
    setResourceProvider(state: State, resourceProviderId: number) {
        state.resourceDetail.resourceProviderId = resourceProviderId;
    },
    removeAudioTranscriptFile(state: State) {
        state.audioDetail.transcriptFile = null;
    },
    removeArticleFile(state: State, fileId: number) {
        state.articleDetail.files = _.filter(state.articleDetail.files, function (f) {
            return f.fileId !== fileId;
        });
    },
    addAuthor(state: State, newAuthor: AuthorModel) {
        state.resourceDetail.resourceAuthors.push(newAuthor);
        setCommonContentState(state);
    },
    removeAuthor(state: State, authorId: number) {
        state.resourceDetail.resourceAuthors = _.filter(state.resourceDetail.resourceAuthors, function (f) {
            return f.id !== authorId;
        });
        setCommonContentState(state);
    },
    addKeyword(state: State, newKeyword: KeywordModel) {
        state.resourceDetail.resourceKeywords.push(newKeyword);
        setCommonContentState(state);
    },
    removeKeyword(state: State, keywordId: number) {
        state.resourceDetail.resourceKeywords = _.filter(state.resourceDetail.resourceKeywords, function (f) {
            return f.id !== keywordId;
        });
        setCommonContentState(state);
    },
    addArticleFile(state: State, file: AttachedFileModel) {
        state.articleDetail.files.push(file);
    },
    replaceArticleFile(state: State, { existingFileId, file }) {
        const index = _.findIndex(state.articleDetail.files, { fileId: existingFileId });
        state.articleDetail.files.splice(index, 1, file);
    },
    saveResourceType(state: State, resourceType: ResourceType) {
        if (state.resourceDetail.resourceType !== resourceType) {
            state.genericFileDetail = new GenericFileResourceModel();
            state.scormDetail = new ScormResourceModel();
            state.imageDetail = new ImageResourceModel();
            state.videoDetail = new VideoResourceModel();
            state.audioDetail = new AudioResourceModel();
            state.articleDetail = new ArticleResourceModel();
            state.weblinkDetail = new WeblinkResourceModel();
        }
        state.resourceDetail.resourceType = resourceType;
        setSpecificContentState(state);
    },
    saveResourceDetail(state: State, { field, value }) {
        Object.assign(state.resourceDetail, {
            [field]: value
        });
        state.commonContentDirty = false;
    },
    saveGenericFileDetail(state: State, { field, value }) {
        Object.assign(state.genericFileDetail, {
            [field]: value
        });
    },
    saveScormDetail(state: State, { field, value }) {
        Object.assign(state.scormDetail, {
            [field]: value
        });
    },
    saveImageDetail(state: State, { field, value }) {
        Object.assign(state.imageDetail, {
            [field]: value
        });
        state.specificContentDirty = false;
    },
    saveVideoDetail(state: State, { field, value }) {
        Object.assign(state.videoDetail, {
            [field]: value
        });
    },
    saveAudioDetail(state: State, { field, value }) {
        Object.assign(state.audioDetail, {
            [field]: value
        });
    },
    saveArticleDetail(state: State, { field, value }) {
        Object.assign(state.articleDetail, {
            [field]: value
        });
        state.specificContentDirty = false;
    },
    saveWeblinkDetail(state: State, { field, value }) {
        Object.assign(state.weblinkDetail, {
            [field]: value
        });
        state.specificContentDirty = false;
    },
    saveHtmlDetail(state: State, { field, value }) {
        Object.assign(state.htmlDetail, {
            [field]: value
        });
    },
} as MutationTree<State>;

const actions = <ActionTree<State, any>>{
    populateUserProviders(context) {
        providerData.getProvidersForUser().then(response => {
            if (response.length > 0) {
                response.unshift({ id: 0, name: 'Not applicable' });
                context.commit("setUserProviders", response);
            }
            else {
                context.commit("setResourceProvider", 0); 
            }
        });
    }
};

const getters = <GetterTree<State, any>>{};

export default new Vuex.Store({
    state,
    mutations,
    actions,
    getters,
    plugins: [autosaverPlugin]
});




import Vue from 'vue';
import axios from 'axios';
import * as _ from "lodash";
import Vuex, { Store } from 'vuex';
import { GetterTree, MutationTree, ActionTree } from "vuex";
import { debounce } from 'ts-debounce';
import { PageSectionDetailModel } from '../models/content/pageSectionDetailModel';
import { FileTypeModel } from '../models/content/fileTypeModel';
import { FileModel, PageSectionDetailFileModel } from '../models/content/fileModel';
import { contentData } from '../data/content';
import { VideoAssetModel } from '../models/content/videoAssetModel';
import { UploadSettingsModel } from '../models/content/uploadSettingsModel';

Vue.use(Vuex); 

export class State {
    saveStatus: string = '';
    isSaving: boolean = false;
    saveError: boolean = false;
    currentUserName: string = '';
    uploadSettings = new UploadSettingsModel();           
    pageSectionDetailId : number;
    fileTypes: FileTypeModel[] = null;
    pageSectionDetail: PageSectionDetailModel = new PageSectionDetailModel();
    isVideoFileValid: boolean;
    isTranscriptFileValid: boolean;
    isCaptionFileValid: boolean;    
    isThumbnailFileValid: boolean;
    contributeAVResourceFlag: boolean;
    learnAVResourceFlag: boolean;
    getAVUnavailableView: string = '';
}
const state = new State();

class ApiRequest {
    data: string;
    action: string;
}

const mutations = {
    async populateUploadSettings(state: State) {
        state.uploadSettings = await contentData.getUploadSettings();
    }, 
    async populateContributeAVResourceFlag(state: State) {
        state.contributeAVResourceFlag = await contentData.getContributeAVResourceFlag();
    },
    async populateAVLearnResourceFlag(state: State) {
        state.learnAVResourceFlag = await contentData.getLearnAVResourceFlag();
    },
    async populateAVUnavailableView(state: State) {
        state.getAVUnavailableView = await contentData.getAVUnavailableView();
    },
    setCurrentUserName(state: State, payload: string) {
        state.currentUserName = payload;
    },
    async setPageSectionDetailId(state: State, payload: number) {
        state.pageSectionDetailId = payload;
    },
    setPageSectionDetail(state: State, pageSectionDetailModel: PageSectionDetailModel) {
        state.pageSectionDetail = pageSectionDetailModel;
        state.pageSectionDetailId = pageSectionDetailModel.id;
    },
    setVideoFile(state: State, file: FileModel) {
        state.pageSectionDetail.videoAsset.videoFile = file;
    },
    setVideoTranscriptFile(state: State, file: FileModel) {
        state.pageSectionDetail.videoAsset.transcriptFile = file;
    },
    setVideoclosedCaptionsFile(state: State, file: FileModel) {
        state.pageSectionDetail.videoAsset.closedCaptionsFile = file;
    },
    setVideoThumbnailImageFile(state: State, file: FileModel) {
        state.pageSectionDetail.videoAsset.thumbnailImageFile = file;
    },
    async removeVideoAttributeFile(state: State, fileTypeId: number) {        
        switch (fileTypeId) {
            case 1:
                state.pageSectionDetail.videoAsset.transcriptFileId = null;
                state.pageSectionDetail.videoAsset.transcriptFile = null;
                break;
            case 2:
                state.pageSectionDetail.videoAsset.closedCaptionsFileId = null;
                state.pageSectionDetail.videoAsset.closedCaptionsFile = null;
                break;
            case 3:
                state.pageSectionDetail.videoAsset.thumbnailImageFileId = null;
                state.pageSectionDetail.videoAsset.thumbnailImageFile = null;                
                break;
        }        
    },    
} as MutationTree<State>;

const actions = <ActionTree<State, any>>{};

const getters = <GetterTree<State, any>>{};

export default new Vuex.Store({
    state,
    mutations,
    actions,
    getters,
});




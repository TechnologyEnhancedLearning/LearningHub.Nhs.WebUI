import axios from 'axios';
import { UploadSettingsModel } from '../models/content/uploadSettingsModel';
import { PageModel } from '../models/content/pageModel';
import { PageResultModel } from '../models/content/pageResultModel';
import { PageSectionDetailModel } from '../models/content/pageSectionDetailModel';
import { DirectionType, PageSectionModel, UpdatePageSectionOrderModel } from '../models/content/pageSectionModel';
import { ResourceType } from '../constants';
import { VideoAssetModel } from '../models/content/videoAssetModel';

const getUploadSettings = async function (): Promise<UploadSettingsModel> {
    return await axios.get<UploadSettingsModel>('/api/content/GetSettings')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getUploadSettings:' + e);
            throw e;
        });
};

const getPages = async function (): Promise<PageResultModel> {

    const url = `/api/content/pages`;

    return await axios.get<PageResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPages:' + e);
            throw e;
        });
};

const getPage = async function (id: number): Promise<PageModel> {

    const url = `/api/content/page/${id}`;

    return await axios.get<PageModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPageById:' + e);
            throw e;
        });
};

const getPageWithAllSections = async function (id: number): Promise<PageModel> {

    const url = `/api/content/page-all/${id}`;

    return await axios.get<PageModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPageWithAllSections:' + e);
            throw e;
        });
};

const getPageSectionDetail = async function (id: number): Promise<PageSectionDetailModel> {

    const url = `/api/content/page-section-detail/${id}`;

    return await axios.get<PageSectionDetailModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPageSectionDetail:' + e);
            throw e;
        });
};

const getPageSectionDetailForEdit = async function (id: number): Promise<PageSectionDetailModel> {

    const url = `/api/content/editable-page-section-detail/${id}`;

    return await axios.get<PageSectionDetailModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPageSectionDetailForEdit:' + e);
            throw e;
        });
};

const discardPageChanges = async function (id: number) {
    const url = `/api/content/discard/${id}`;
    return await axios.put(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('discard:' + e);
            throw e;
        });
};

const publishPageChanges = async function (id: number) {
    const url = `/api/content/publish/${id}`;
    return await axios.put(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('publishPageChanges:' + e);
            throw e;
        });
};

const changeOrder = async function (id: number, sectionId:number, direction : DirectionType) {
    const url = `/api/content/change-order`;
    let updatePageSectionOrderModel = new UpdatePageSectionOrderModel({
        pageId : id,
        pageSectionId : sectionId,
        directionType : direction
    });
    return await axios.put(url, updatePageSectionOrderModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('changeOrder:' + e);
            throw e;
        });
};

const cloneSection = async function (pageSectionId: number) {
    const url = `/api/content/clone/${pageSectionId}`;
    return await axios.put(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('cloneSection:' + e);
            throw e;
        });
};

const hideSection = async function (pageSectionId: number) {
    const url = `/api/content/hide/${pageSectionId}`;
    return await axios.put(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('cloneSection:' + e);
            throw e;
        });
};

const unHideSection = async function (pageSectionId: number) {
    const url = `/api/content/unhide/${pageSectionId}`;
    return await axios.put(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('cloneSection:' + e);
            throw e;
        });
};

const deleteSection = async function (pageSectionId: number) {
    const url = `/api/content/delete/${pageSectionId}`;
    return await axios.put(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('cloneSection:' + e);
            throw e;
        });
};

const updatePageImageSectionDetail = async function (pageId: number, data:FormData): Promise<boolean> {

    const url = `/api/content/page-image-section-detail/${pageId}`;

    return await axios.post<boolean>(url, data)
        .then(_ => {
            return true;
        })
        .catch(e => {
            console.log('updatePageImageSectionDetail:' + e);
            throw e;
        });
};

const createPageSection = async function (pageSection: PageSectionModel): Promise<PageSectionDetailModel> {

    const url = `/api/content/create-page-section`;

    return await axios.post(url, pageSection)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('create-page-section:' + e);
            throw e;
        });
};

const updatePageSectionDetail = async function (pageSection: PageSectionDetailModel): Promise<boolean> {

    const url = `/api/content/update-page-section-detail`;

    return await axios.put(url, pageSection)
        .then(response => {
            return true;
        })
        .catch(e => {
            console.log('updatePageSectionDetail:' + e);
            throw e;
        });
};

const getPageSectionDetailVideo = async function (pageSectionDetailId: number): Promise<PageSectionDetailModel> {
    return await axios.get<PageSectionDetailModel>('/api/content/page-section-detail-video/' + pageSectionDetailId)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPageSectionDetailVideo:' + e);
            throw e;
        });
};

const updateVideoAsset = async function (videoAsset: VideoAssetModel): Promise<boolean> {    
    return await axios.post<boolean>('/api/content/update-video-asset', videoAsset)
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('deleteAttachedFile:' + e);
            throw e;
        });
};

const getAddAVFlag = async function (): Promise<boolean> {
    return await axios.get<boolean>('/Resource/GetAddAVFlag')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getAddAVFlag:' + e);
            throw e;
        });
};

const getDisplayAVFlag = async function (): Promise<boolean> {
    return await axios.get<boolean>('/Resource/GetDisplayAVFlag')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getDisplayAVFlag:' + e);
            throw e;
        });
};

const getAVUnavailableView = async function (): Promise<string> {
    return await axios.get('/Resource/GetAVUnavailableView')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.error('Error fetching shared partial view:', e)
            throw e;
        });
};

export const contentData = {
    getUploadSettings,
    getPages,
    getPage,
    getPageWithAllSections,
    discardPageChanges,
    publishPageChanges,
    getPageSectionDetail,
    getPageSectionDetailForEdit,
    updatePageImageSectionDetail,    
    changeOrder,
    cloneSection,
    hideSection,
    unHideSection,
    deleteSection,
    createPageSection,
    updatePageSectionDetail,
    getPageSectionDetailVideo,
    updateVideoAsset,
    getAddAVFlag,
    getDisplayAVFlag,
    getAVUnavailableView
};
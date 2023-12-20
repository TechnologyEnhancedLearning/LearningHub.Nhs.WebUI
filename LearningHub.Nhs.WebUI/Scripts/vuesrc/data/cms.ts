import AxiosWrapper from '../axiosWrapper';
import { PageModel } from '../models/cms/pageModel';
import { PageSectionDetailModel } from '../models/cms/pageSectionDetailModel';

const getPage = async function (id: number, preview: boolean = false): Promise<PageModel> {

    let url = `/api/content/page/${id}`;
    if (preview) url += `?preview=${preview}`

    return await AxiosWrapper.axios.get<PageModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPage:' + e);
            throw e;
        });
};

const getPageSectionDetailVideo = async function (pageSectionDetailId: number): Promise<PageSectionDetailModel> {

    return await AxiosWrapper.axios.get<PageSectionDetailModel>('/api/content/page-section-detail-video/' + pageSectionDetailId)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getPageSectionDetailVideo:' + e);
            throw e;
        });
};

export const contentData = {
    getPage,
    getPageSectionDetailVideo
};
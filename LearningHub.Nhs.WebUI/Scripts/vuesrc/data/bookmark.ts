import AxiosWrapper from '../axiosWrapper';
import { UserBookmarkModel } from '../models/userBookmark';

const toggle = async function (bookmark: UserBookmarkModel): Promise<number> {    
    return await AxiosWrapper.axios.put<number>('/api/bookmark/toggle', bookmark)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('toggleBookmark:' + e);
            throw e;
        });
};

const deleteFolder = async function (bookmarkId: number): Promise<boolean> {
    return await AxiosWrapper.axios.delete<boolean>(`/api/bookmark/deletefolder/${bookmarkId}`)
        .then(response => {
            return true;
        })
        .catch(e => {
            console.log('deleteFolder:' + e);
            throw e;
        });
};

const create = async function (bookmark: UserBookmarkModel): Promise<number> {
    return await AxiosWrapper.axios.put<number>('/api/bookmark/create', bookmark)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('createFolder:' + e);
            throw e;
        });
};

const edit = async function (bookmark: UserBookmarkModel): Promise<number> {
    return await AxiosWrapper.axios.put<number>('/api/bookmark/edit', bookmark)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('createFolder:' + e);
            throw e;
        });
};

const getBookmarks = async function (parentId?: number): Promise<UserBookmarkModel[]> {
   
    const url =  (parentId > 0) ?
        `/api/bookmark/GetAllByParent/${parentId}?timestamp=${new Date().getTime()}`
        : `/api/bookmark/GetAllByParent`;

    return await AxiosWrapper.axios.get<UserBookmarkModel[]>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('GetAllByParent:' + e);
            throw e;
        });
};

const getHelpUrl = async function (): Promise<string> {
    const url = `/api/bookmark/getHelpUrl`;
    return await AxiosWrapper.axios.get<string>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getHelpUrl:' + e);
            throw e;
        });
};

export const bookmarkData = {
    toggle,
    getBookmarks,
    create,
    edit,
    deleteFolder,
    getHelpUrl
}

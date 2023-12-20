import AxiosWrapper from '../axiosWrapper';

export const url_is_accessible = async function (url: string): Promise<boolean> {
    if (!url) {
        return true;
    }
    const params = {
        url: encodeURIComponent(url)
    }
    return await AxiosWrapper.axios.post<boolean>('/api/Contribute/UrlIsAccessible', params)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            return false;
        });
};
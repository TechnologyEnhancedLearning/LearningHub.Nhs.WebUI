import AxiosWrapper from '../axiosWrapper';
import { ProviderModel } from '../models/ProviderModel';
const getProviders = async function (): Promise<ProviderModel[]> {
    return await AxiosWrapper.axios.get<ProviderModel[]>('/api/Provider/GetProviders')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getProviders:' + e);
            throw e;
        });
};

const getProvidersForUser = async function (): Promise<ProviderModel[]> {
    return await AxiosWrapper.axios.get<ProviderModel[]>('/api/Provider/GetProvidersForUser')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getProvidersForUser:' + e);
            throw e;
        });
};

export const providerData = {
    getProviders,
    getProvidersForUser
}

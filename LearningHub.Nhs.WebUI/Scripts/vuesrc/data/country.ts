import AxiosWrapper from '../axiosWrapper';
import { GenericListItemModel } from '../models/genericListItemModel';

const getFilteredCountries = async function (filter: String): Promise<GenericListItemModel[]> {
    return await AxiosWrapper.axios.get<GenericListItemModel[]>('/api/Country/GetFiltered/' + filter)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getFilteredCountries:' + e);
            throw e;
        });
};

const getCountry = async function (id: Number): Promise<GenericListItemModel> {
    return await AxiosWrapper.axios.get<GenericListItemModel>('/api/Country/GetById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getCountry:' + e);
            throw e;
        });
};

export const countryData = {
    getFilteredCountries,
    getCountry
};

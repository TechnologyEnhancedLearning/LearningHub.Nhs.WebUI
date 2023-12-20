import AxiosWrapper from '../axiosWrapper';
import { LocationModel } from '../models/locationModel';

const getFilteredLocations = async function (criteria: string): Promise<LocationModel[]> {
    return await AxiosWrapper.axios.get<LocationModel[]>('/api/Location/GetFiltered/' + criteria)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getFilteredLocation:' + e);
            throw e;
        });
    
};
const getLocation = async function (criteria: string): Promise<LocationModel> {
    return await AxiosWrapper.axios.get<LocationModel>('/api/Location/GetById/' + criteria)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getFilteredLocation:' + e);
            throw e;
        });
};

export const locationData = {
    getFilteredLocations,
    getLocation
};

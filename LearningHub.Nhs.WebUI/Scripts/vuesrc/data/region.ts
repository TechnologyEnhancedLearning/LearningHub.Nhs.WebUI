import AxiosWrapper from '../axiosWrapper';
import { GenericListItemModel } from '../models/genericListItemModel';


const getAllRegions = async function (): Promise<GenericListItemModel[]> {
    return await AxiosWrapper.axios.get<GenericListItemModel[]>('/api/Region/GetAll')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getAllRegions:' + e);
            throw e;
        });
};

export const regionData = {
    getAllRegions
}; 

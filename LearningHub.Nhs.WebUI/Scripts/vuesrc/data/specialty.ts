import AxiosWrapper from '../axiosWrapper';
import { GenericListItemModel } from '../models/genericListItemModel';

const getSpecialties = async function (): Promise<GenericListItemModel[]> {
    return await AxiosWrapper.axios.get<GenericListItemModel[]>('/api/Specialty/GetSpecialties')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getSpecialties:' + e);
            throw e;
        });
};

export const specialtyData = {
    getSpecialties
};

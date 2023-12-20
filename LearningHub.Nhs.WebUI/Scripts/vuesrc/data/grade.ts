import AxiosWrapper from '../axiosWrapper';
import { GenericListItemModel } from '../models/genericListItemModel';

const getGradesForJobRole = async function (jobRoleId: number): Promise<GenericListItemModel[]> {
    return await AxiosWrapper.axios.get<GenericListItemModel[]>('/api/Grade/GetGradesForJobRole/' + jobRoleId)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getGradesForJobRole:' + e);
            throw e;
        });
};

export const gradeData = {
    getGradesForJobRole
};

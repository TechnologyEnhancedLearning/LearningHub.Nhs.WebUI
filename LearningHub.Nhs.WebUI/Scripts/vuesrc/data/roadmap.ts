import AxiosWrapper from '../axiosWrapper';
import {RoadMapResultModel } from '../models/roadmap';

const getUpdates = async function (noOfRecords: number): Promise<RoadMapResultModel> {    
    return await AxiosWrapper.axios.get<RoadMapResultModel>('/api/roadmap/updates/' + noOfRecords)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getUpdates:' + e);
            throw e;
        });
};

export const roadMapData = {
    getUpdates
};
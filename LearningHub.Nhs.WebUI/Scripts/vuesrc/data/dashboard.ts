import axios from 'axios';
import { CatalogueCardsResultModel } from '../models/dashboardModel';
import { LearningHubValidationResultModel } from '../models/learningHubValidationResultModel';

const getCatalogues = async function (dashboardType: string, fetchRows: number = 10, offsetRows: number = 0): Promise<CatalogueCardsResultModel> {

    const url = `/api/dashboard/catalogues/${dashboardType}/${fetchRows}/${offsetRows}?timestamp=${new Date().getTime()}`;

    return await axios.get<CatalogueCardsResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getHistory:' + e);
            throw e;
        });
};

const recordDashboardEvent = async function (eventType: string, resourceReference: number = null){

    const url = `/api/dashboard/recordevent`;

    var data = {
        eventType: eventType,
        resourceReference: resourceReference,        
    };

    return await axios.post<LearningHubValidationResultModel>(url, data)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            throw e;
        });
};

export const dashboardData = {
    getCatalogues,
    recordDashboardEvent
};
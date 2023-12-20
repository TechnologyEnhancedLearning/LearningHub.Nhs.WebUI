import AxiosWrapper from '../axiosWrapper';
import { RatingSummaryModel } from '../models/ratingSummaryModel';
import { LearningHubValidationResultModel } from '../models/learningHubValidationResultModel';

const getRatingSummary = async function (entityVersionId: number): Promise<RatingSummaryModel> {
    return await AxiosWrapper.axios.get<RatingSummaryModel>('/api/Rating/GetRatingSummary/' + entityVersionId + `?timestamp=${new Date().getTime()}`)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getRatingSummary:' + e);
            throw e;
        });
};

const createRating = async function (resourceVersionId: number, rating: number): Promise<LearningHubValidationResultModel> {
    const params = {
        entityVersionId: resourceVersionId,
        rating: rating
    }
    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/Rating/CreateRating', params)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('createRating:' + e);
            throw e;
        });
};

const updateRating = async function (resourceVersionId: number, rating: number): Promise<LearningHubValidationResultModel> {
    const params = {
        entityVersionId: resourceVersionId,
        rating: rating
    }
    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/Rating/UpdateRating', params)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('updateRating:' + e);
            throw e;
        });
};

export const ratingData = {
    getRatingSummary,
    createRating,
    updateRating
}

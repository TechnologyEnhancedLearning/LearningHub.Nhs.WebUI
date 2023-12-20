import AxiosWrapper from '../../axiosWrapper';
import { AssessmentProgressModel } from '../../models/mylearning/assessmentProgressModel';

const getAssessmentProgressFromActivity = async function (
    assessmentResourceActivityId: number): Promise<AssessmentProgressModel> {
        
    return await AxiosWrapper.axios.get<AssessmentProgressModel>('/api/resource/GetAssessmentProgress/activity/' + assessmentResourceActivityId)
        .then(response => {
            if (!response.data) {
                window.location.pathname = './Home/Error';
            }
            return new AssessmentProgressModel(response.data);
        })
        .catch(e => {
            console.log('getAssessmentProgressFromActivity:' + e);
            throw e;
        });
};

const getAssessmentProgressFromResourceVersion = async function (
    resourceVersionId: number): Promise<AssessmentProgressModel> {
        
    return await AxiosWrapper.axios.get<AssessmentProgressModel>('/api/resource/GetAssessmentProgress/resource/' + resourceVersionId)
        .then(response => {
            if (response.status === 204) {
                return null
            }
            else if (response.status !== 200) {
                window.location.pathname = './Home/Error';
            }
            return new AssessmentProgressModel(response.data);
        })
        .catch(e => {
            console.log('getAssessmentProgressFromResourceVersion:' + e);
            throw e;
        });
};


export const assessmentResourceHelper = {
    getAssessmentProgressFromActivity,
    getAssessmentProgressFromResourceVersion,
};

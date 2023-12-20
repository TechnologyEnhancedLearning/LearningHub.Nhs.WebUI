import AxiosWrapper from '../axiosWrapper';
import { CaseResourceModel } from '../models/contribute-resource/caseResourceModel';
import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
import { NewPartialFileRequestModel } from '../models/contribute-resource/files/newPartialFileRequestModel';
import { AssessmentModel } from '../models/contribute-resource/assessmentModel';

const saveResourceDetail = async function (caseDetails: ContributeResourceDetailModel): Promise<any> {
    return AxiosWrapper.axios.post('/api/Contribute/saveResourceDetail', caseDetails)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('contributeApi.saveResourceDetail:' + e);
            throw e;
        });
};

const saveCaseDetail = async function (caseDetails: CaseResourceModel): Promise<any> {
    return AxiosWrapper.axios.post('/api/Contribute/SaveCaseDetail', caseDetails)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('contributeApi.saveCaseDetail:' + e);
            throw e;
        });
};

const saveAssessmentDetail = async function (assessmentDetails: AssessmentModel): Promise<any> {
    return AxiosWrapper.axios.post('/api/Contribute/SaveAssessmentDetail', assessmentDetails)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('contributeApi.saveAssessmentDetail:' + e);
            throw e;
        });
};

const createPartialFile = async function (partialFile: NewPartialFileRequestModel): Promise<any> {
    return AxiosWrapper.axios.post('/api/Contribute/CreatePartialFile', partialFile)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('contributeApi.createPartialFile:' + e);
            throw e;
        });
};

export default {
    saveResourceDetail,
    saveCaseDetail,
    saveAssessmentDetail,
    createPartialFile,
};

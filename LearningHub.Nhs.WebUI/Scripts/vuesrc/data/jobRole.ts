import AxiosWrapper from '../axiosWrapper';
import { JobRoleBasicModel } from '../models/jobRoleBasicModel';

const getFilteredJobRoles = async function (filter: string): Promise<JobRoleBasicModel[]> {
    return await AxiosWrapper.axios.get<JobRoleBasicModel[]>('/api/JobRole/GetFiltered/' + filter)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getFilteredJobRoles:' + e);
            throw e;
        });
};

const getJobRole = async function (id: number): Promise<JobRoleBasicModel> {
    return await AxiosWrapper.axios.get<JobRoleBasicModel>('/api/JobRole/GetById/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getJobRole:' + e);
            throw e;
        });
};

const validateMedicalCouncilNumber = async function (lastName : string, medicalCouncilId: number, medicalCouncilNumber: string): Promise<string> {
    return await AxiosWrapper.axios.get<string>('/api/JobRole/ValidateMedicalCouncilNumber/' + lastName + '/' + medicalCouncilId + '/' + medicalCouncilNumber + '/')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('validateMedicalCouncilNumber:' + e);
            throw e;
        });
};

export const jobRoleData = {
    getFilteredJobRoles,
    getJobRole,
    validateMedicalCouncilNumber
};

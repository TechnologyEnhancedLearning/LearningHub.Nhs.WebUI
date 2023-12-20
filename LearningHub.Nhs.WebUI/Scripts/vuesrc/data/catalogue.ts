import AxiosWrapper from '../axiosWrapper';
import { CatalogueModel, CatalogueResourceResultModel } from '../models/catalogueModel';
import { RestrictedCatalogueAccessRequestsRequestModel } from '../models/catalogue/restrictedCatalogueAccessRequestsRequestModel';
import { RestrictedCatalogueAccessRequestModel } from '../models/catalogue/restrictedCatalogueAccessRequestModel';
import { RestrictedCatalogueUsersRequestModel } from '../models/catalogue/restrictedCatalogueUsersRequestModel';
import { RestrictedCatalogueUsersModel } from '../models/catalogue/restrictedCatalogueUsersModel';
import { RestrictedCatalogueSummaryModel } from '../models/catalogue/RestrictedCatalogueSummaryModel';
import { RestrictedCatalogueInviteUserRequestModel } from '../models/catalogue/restrictedCatalogueInviteUserRequestModel';
import { CatalogueAccessRequestModel } from '../models/catalogue/catalogueAccessRequestModel';

const getCatalogue = async function (reference: string): Promise<CatalogueModel> {
    return await AxiosWrapper.axios.get<CatalogueModel>('/api/catalogue/' + reference + `?timestamp=${new Date().getTime()}`)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.error('getCatalogue:' + e);
            throw e;
        });
};

const getCatalogueRecorded = async function (reference: string): Promise<CatalogueModel> {
    console.log('ref: ' + reference);
    return await AxiosWrapper.axios.get<CatalogueModel>('/api/catalogue-recorded/' + reference + `?timestamp=${new Date().getTime()}`)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.error('getCatalogueRecorded:' + e);
            throw e;
        });
};

const getCatalogueResources = async function (nodeId: number, catalogueOrderEnum: string, offset: number): Promise<CatalogueResourceResultModel> {
    const url = `/api/catalogue/resources/${nodeId}/${catalogueOrderEnum}/${offset}` + `?timestamp=${new Date().getTime()}`;
    return await AxiosWrapper.axios.get<CatalogueResourceResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.error('getCatalogueResources:' + e);
            throw e;
        });
};

const sendAccessRequest = async function (reference: string, model: any) {
    const url = `/api/catalogue/requestaccess/${reference}`;
    return await AxiosWrapper.axios.post<any>(url, model)
        .then(response => {

        })
        .catch(e => {
            console.error('sendaccessrequest:' + e);
            throw e;
        })
}

const getAccessRequests = async function (requestModel: RestrictedCatalogueAccessRequestsRequestModel): Promise<RestrictedCatalogueAccessRequestModel[]> {
    return await AxiosWrapper.axios.post<RestrictedCatalogueAccessRequestModel[]>('/api/catalogue/GetRestrictedCatalogueAccessRequests', requestModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getaccessrequests:' + e);
            throw e;
        });
}

const getSummary = async function (catalogueNodeId: number): Promise<RestrictedCatalogueSummaryModel> {
    const url = `/api/catalogue/GetRestrictedCatalogueSummary/${catalogueNodeId}` + `?timestamp=${new Date().getTime()}`;
    return await AxiosWrapper.axios.get<RestrictedCatalogueSummaryModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getSummary:' + e);
            throw e;
        });
}

const getUsers = async function (requestModel: RestrictedCatalogueUsersRequestModel): Promise<RestrictedCatalogueUsersModel> {
    return await AxiosWrapper.axios.post<RestrictedCatalogueUsersModel>('/api/catalogue/GetRestrictedCatalogueUsers', requestModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getUsers:' + e);
            throw e;
        });
}

const inviteUser = async function (requestModel: RestrictedCatalogueInviteUserRequestModel) {
    return await AxiosWrapper.axios.post<any>('/api/catalogue/InviteUser', requestModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('inviteUser:' + e);
            throw e;
        });
}

const getAccessDetailsForCatalogue = async function (reference: string) {
    const url = `/api/catalogue/getaccessdetails/${reference}` + `?timestamp=${new Date().getTime()}`;
    return await AxiosWrapper.axios.get<any>(url)
        .then(response => response.data)
        .catch(e => {
            console.error('getaccess: ' + e);
            throw e;
        });
}

const getLatestCatalogueAccessRequest = async function (catalogueNodeId: number) {
    const url = `/api/catalogue/GetLatestCatalogueAccessRequest/${catalogueNodeId}` + `?timestamp=${new Date().getTime()}`;
    return await AxiosWrapper.axios.get<any>(url)
        .then(response => response.data)
        .catch(e => {
            console.error('getlatestcatalogueaccess: ' + e);
            throw e;
        });
}

const removeUserFromRestrictedAccess = async function (userUserGroupId: number) {
    const url = `/api/catalogue/removeUserFromRestrictedAccessUserGroup/${userUserGroupId}`;
    return await AxiosWrapper.axios.post<any>(url);
}

const dismissAccessRequest = async function (catalogueNodeId: number) {
    const url = `/api/catalogue/dismissaccessrequest/${catalogueNodeId}`;
    return await AxiosWrapper.axios.post<any>(url);
}

const acceptAccessRequest = async function (accessRequest: any) {
    const url = `/api/catalogue/acceptAccessRequest`;
    return await AxiosWrapper.axios.post<any>(url, accessRequest);
}

const rejectAccessRequest = async function (accessRequestId: number, rejectionReason: string) {
    const url = `/api/catalogue/rejectAccessRequest/${accessRequestId}`;
    return await AxiosWrapper.axios.post<any>(url, {rejectionReason});
}

const getAccessRequest = async function (accessRequestId: number) {
    const url = `/api/catalogue/accessRequest/${accessRequestId}` + `?timestamp=${new Date().getTime()}`;    
    return await AxiosWrapper.axios.get<any>(url)
        .then(response => response.data)
        .catch(e => {
            console.error('getaccessrequest: ' + e);
            throw e;
        });
}

export const catalogueData = {
    getCatalogue,
    getCatalogueRecorded,
    getCatalogueResources,
    sendAccessRequest,
    getAccessRequests,
    getUsers,
    getSummary,
    getAccessDetailsForCatalogue,
    getLatestCatalogueAccessRequest,
    acceptAccessRequest,
    rejectAccessRequest,
    removeUserFromRestrictedAccess,
    inviteUser,
    dismissAccessRequest,
    getAccessRequest
};
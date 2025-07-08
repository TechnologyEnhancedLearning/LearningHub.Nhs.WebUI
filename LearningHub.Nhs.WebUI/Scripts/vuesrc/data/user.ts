import AxiosWrapper from '../axiosWrapper';
import { ActiveContentModel } from '../models/activeContentModel';
import { UserModel } from '../models/userModel';
import { RoleUserGroupModel } from '../models/roleUserGroupModel';
import { UserBasicModel, UserPersonalDetailsModel, UserSecurityQuestionAnswerModel, UserSecurityQuestion } from '../models/userBasicModel';

const getCurrentUser = async function (): Promise<UserModel>{
    var url = '/api/User/Current';
    return await AxiosWrapper.axios.get<UserModel>(url)
        .then(x => x.data)
        .catch(x => { throw new Error("getcurrentuser: " + x) });
}

const getCurrentUserProfile = async function (): Promise<any> {
    var url = `/api/User/CurrentProfile`;
    return await AxiosWrapper.axios.get<UserModel>(url)
        .then(x => x.data)
        .catch(x => { throw new Error("getcurrentuserprofile: " + x) });
}

const getActiveContent = async function (): Promise<ActiveContentModel[]> {
    var url = `/api/User/GetActiveContent`;
    return await AxiosWrapper.axios.get<ActiveContentModel[]>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getActiveContent:' + e);
            throw e;
        });
};

const getRoleUserGroups = async function (userId: number | null = null): Promise<RoleUserGroupModel[]> {
    var url = `/api/UserGroup/GetRoleUserGroupDetail/${userId ? userId : ""}?timestamp=${new Date().getTime()}`;
    return await AxiosWrapper.axios.get<RoleUserGroupModel[]>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getRoleUserGroups:' + e);
            throw e;
        });
};

const isGeneralUser = async function (): Promise<boolean[]> {
    var isGeneralUser = `/api/User/GetUserAccessType`;
    return await AxiosWrapper.axios.get<boolean[]>(isGeneralUser)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('isGeneralUser:' + e);
            throw e;
        });
};

const IsSystemAdmin = async function (): Promise<boolean[]> {
    var IsSystemAdmin = `/api/User/CheckUserRole`;
    return await AxiosWrapper.axios.get<boolean[]>(IsSystemAdmin)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('IsSystemAdmin:' + e);
            throw e;
        });
};

const IsValidUser = async function (currentPassword: string): Promise<boolean[]> {
    var IsValidUser = `/api/User/ConfirmPassword/${currentPassword}`;
    return await AxiosWrapper.axios.get<boolean[]>(IsValidUser)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('IsValidUser:' + e);
            throw e;
        });
};

const getCurrentUserBasicDetails = async function (): Promise<UserBasicModel> {
    return await AxiosWrapper.axios.get<UserBasicModel>('/api/User/GetCurrentUserBasicDetails')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getCurrentUserBasicDetails:' + e);
            throw e;
        });
};

const getCurrentUserPersonalDetails = async function (): Promise<UserPersonalDetailsModel> {
    return await AxiosWrapper.axios.get<UserPersonalDetailsModel>('/api/User/GetCurrentUserPersonalDetails')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('GetCurrentUserPersonalDetails:' + e);
            throw e;
        });
};

const updatePersonalDetails = async function (userPersonalDetails: UserPersonalDetailsModel): Promise<UserPersonalDetailsModel> {
    return await AxiosWrapper.axios.post<UserPersonalDetailsModel>('/api/User/UpdatePersonalDetails', userPersonalDetails)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('updatePersonalDetails:' + e);
            throw e;
        });
};

const changePassword = async function (newPassword: string, confirmPassword: string): Promise<boolean> {
    const params = {
        newPassword: newPassword,
        passwordConfirmation: confirmPassword
    }
    return await AxiosWrapper.axios.post<UserPersonalDetailsModel>('/api/User/ChangePassword', params)
        .then(response => {
            return true;
        })
        .catch(e => {
            console.log('changePassword:' + e);
            throw e;
        });
};


const getSecurityQuestionAnswers = async function (): Promise<UserSecurityQuestionAnswerModel> {
    return await AxiosWrapper.axios.get<UserSecurityQuestionAnswerModel>('/api/User/GetSecurityQuestions')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getSecurityQuestions:' + e);
            throw e;
        });
};

const updateSecurityQuestionAnswers = async function (securityQuestions: UserSecurityQuestion[]): Promise<UserPersonalDetailsModel> {
    return await AxiosWrapper.axios.post<UserPersonalDetailsModel>('/api/User/UpdateSecurityQuestions', securityQuestions)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('updateSecurityQuestionAnswers:' + e);
            throw e;
        });
};

const keepUserSessionAlive = async function (): Promise<boolean> {
    return await AxiosWrapper.axios.post<boolean>('/api/User/KeepUserSessionAlive')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('KeepUserSessionAlive:' + e);
            throw e;
        });
};

const getkeepUserSessionAliveInterval = async function (): Promise<number> {
    return await AxiosWrapper.axios.get<number>('/api/User/GetkeepUserSessionAliveInterval')
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getkeepUserSessionAliveInterval:' + e);
            throw e;
        });
};

export const userData = {
    getCurrentUser,
    getCurrentUserProfile,
    getRoleUserGroups,
    getCurrentUserBasicDetails,
    getActiveContent,
    getCurrentUserPersonalDetails,
    updatePersonalDetails,   
    changePassword,
    getSecurityQuestionAnswers,
    updateSecurityQuestionAnswers,
    keepUserSessionAlive,
    getkeepUserSessionAliveInterval,
    isGeneralUser,
    IsSystemAdmin,
    IsValidUser
}

import Vue from 'vue';
import Vuex from 'vuex';
import AxiosWrapper from '../axiosWrapper';
import { GetterTree, MutationTree, ActionTree } from "vuex";

Vue.use(Vuex);

export class State {
    userObj: User;
}

export class User {
    emailAddress: string = '';
    firstName: string = '';
    lastName: string = '';
    countryId: number = 0;
    countryName: string = '';
    regionId: number = null;
    regionName: string = '';
    preferredName: string = '';
    secondaryEmailAddress: string = '';
    jobRoleId: number = 0;
    jobRoleName: string = '';
    gradeId: number = 0;
    gradeName: string = '';
    medicalCouncilId: number = 0;
    medicalCouncilName: string = '';
    medicalCouncilCode: string = '';
    medicalCouncilNumber: string = '';
    specialtyId: string = '';
    specialtyName: string = '';
    locationStartDate: string = '';
    locationId: number = 0;
    locationName: ''
}

const state = new State();
state.userObj = new User();

const mutations = <MutationTree<State>>{
    setEmailAddress(state: State, payload: string) {
        state.userObj.emailAddress = payload;
    },
    setPersonalInfo(state: State, payload: any) {
        state.userObj.firstName = payload.firstName;
        state.userObj.lastName = payload.lastName;
        state.userObj.countryId = payload.countryId;
        state.userObj.countryName = payload.countryName;
        state.userObj.regionId = payload.regionId;
        state.userObj.regionName = payload.regionName;
        state.userObj.preferredName = payload.preferredName;
        state.userObj.secondaryEmailAddress = payload.secondaryEmailAddress;
    },
    setJobRoleInfo(state: State, payload: any) {
        state.userObj.jobRoleId = payload.jobRoleId;
        state.userObj.jobRoleName = payload.jobRoleName;
        state.userObj.gradeId = payload.gradeId;
        state.userObj.gradeName = payload.gradeName;

        state.userObj.medicalCouncilId = payload.medicalCouncilId;
        state.userObj.medicalCouncilName = payload.medicalCouncilName;
        state.userObj.medicalCouncilCode = payload.medicalCouncilCode;
        state.userObj.medicalCouncilNumber = payload.medicalCouncilNumber;
        state.userObj.specialtyId = payload.specialtyId;
        state.userObj.specialtyName = payload.specialtyName;
    },
    setPlaceOfWorkInfo(state: any, payload: any) {
        state.userObj.locationStartDate = payload.startDate;
        state.userObj.locationId = payload.locationId;
        state.userObj.locationName = payload.locationName;
    }
};
const actions = <ActionTree<State, any>>{
    async processRegistrationDetails(context) {
        const params = {
            emailAddress: context.state.userObj.emailAddress,
            firstName: context.state.userObj.firstName,
            lastName: context.state.userObj.lastName,
            countryId: context.state.userObj.countryId,
            regionId: context.state.userObj.regionId,
            preferredName: context.state.userObj.preferredName,
            secondaryEmailAddress: context.state.userObj.secondaryEmailAddress,
            jobRoleId: context.state.userObj.jobRoleId,
            gradeId: context.state.userObj.gradeId,
            medicalCouncilNumber: context.state.userObj.medicalCouncilNumber,
            specialtyId: context.state.userObj.specialtyId,
            locationStartDate: context.state.userObj.locationStartDate,
            locationId: context.state.userObj.locationId
        };

        return await AxiosWrapper.axios.post('/api/Registration/PostAsync', params)
            .then(response => {
                return response.data;
            })
            .catch(e => {
                console.log('processRegistrationDetails:' + e);
            });
    }
};
const getters = <GetterTree<State, any>>{};

export default new Vuex.Store({
    state,
    mutations,
    actions,
    getters
});

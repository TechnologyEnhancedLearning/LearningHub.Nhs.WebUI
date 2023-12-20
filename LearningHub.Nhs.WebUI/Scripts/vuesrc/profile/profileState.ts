import Vue from 'vue';
import Vuex from 'vuex';
import AxiosWrapper from '../axiosWrapper';
import { GetterTree, MutationTree, ActionTree } from "vuex";
import { UserBasicModel } from '../models/userBasicModel';
Vue.use(Vuex);

export class State {
    user: UserBasicModel;
}

const state = new State();
state.user = new UserBasicModel();
const mutations = <MutationTree<State>>{
    setUser(state: State, payload: UserBasicModel) {
        state.user.id = payload.id;
        state.user.userName = payload.userName;
        state.user.firstName = payload.firstName;
        state.user.lastName = payload.lastName;
        state.user.lastUpdated = payload.lastUpdated;
    },
};

const actions = <ActionTree<State, any>>{
  
};

const getters = <GetterTree<State, any>>{
    user(state) {
        return state.user;
    }
};

export default new Vuex.Store({
    state,
    mutations,
    actions,
    getters
});

import Vue from 'vue';
import Vuex from 'vuex';
import { GetterTree, MutationTree, ActionTree } from "vuex";
import { ResourceHeaderModel } from '../models/resourceHeaderModel';
import { ResourceItemModel } from '../models/resourceItemModel';
import { ResourceInformationModel } from '../models/resourceInformationModel';
import { ResourceLocationsModel } from '../models/resourceLocationsModel';

Vue.use(Vuex);

export class State {
    resourceHeaderObj: ResourceHeaderModel;
    resourceInformationObj: ResourceInformationModel;
    resourceItemObj: ResourceItemModel;
    resourceLocationObj: ResourceLocationsModel;
    unableToViewELearningResources: string = '';
}

const state = new State();
state.resourceHeaderObj = new ResourceHeaderModel();
state.resourceInformationObj = new ResourceInformationModel({ id: 0 });
state.resourceItemObj = new ResourceItemModel({ id: 0 });
state.resourceLocationObj = new ResourceLocationsModel({ id: 0 });

const mutations = <MutationTree<State>> {
    setHeader(state: State, payload: ResourceHeaderModel) {
        state.resourceHeaderObj = payload;
    },
    setResourceInfo(state: State, payload: ResourceInformationModel) {
        state.resourceInformationObj = payload;
    },
    setResourceItem(state: State, payload: ResourceItemModel) {
        state.resourceItemObj = payload;
    },
    setResourceLocations(state: State, payload: ResourceLocationsModel) {
        state.resourceLocationObj = payload;
    },
    setUnableToViewELearningResources(state: State, payload: string) {
        state.unableToViewELearningResources = payload;
    }
};
const actions = <ActionTree<State, any>>{};
const getters = <GetterTree<State, any>>{};

export default new Vuex.Store({
    state,
    mutations,
    actions,
    getters
});

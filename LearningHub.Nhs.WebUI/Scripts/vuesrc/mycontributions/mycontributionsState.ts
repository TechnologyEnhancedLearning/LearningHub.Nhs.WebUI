import Vue from 'vue';
import AxiosWrapper from '../axiosWrapper';
import * as _ from "lodash";
import Vuex, { Store, Module, ActionContext } from 'vuex';
import { GetterTree, MutationTree, ActionTree, ModuleTree } from "vuex";
import { resourceData } from '../data/resource';
import { CatalogueBasicModel } from '../models/catalogueModel';
import { MyContributeTabEnum } from './mycontributionsEnum'
import { MyContributionsCardModel } from '../models/contribute/mycontributionsCardModel';
import { contentStructureState } from '../content-structure-editor/contentStructureState';
import { VersionStatus } from '../constants';

Vue.use(Vuex); 

export class State {
    isReadonly = true;
    cardsLoaded = false;
    cardTotalsLoaded = false;
    userCatalogues: CatalogueBasicModel[] = null;
    selectedCatalogueId = 0;
    initialCatalogueUrl = '';
    selectedCatalogue: CatalogueBasicModel = null;
    cards: MyContributionsCardModel[] = null;
    actionRequiredCount = 0;
    draftCount = 0;
    publishedCount = 0;
    unpublishedCount = 0;
    userActionRequiredCount = 0;
    userDraftCount = 0;
    userPublishedCount = 0;
    userUnpublishedCount = 0;
    selectedTab = MyContributeTabEnum.Unspecified;
    restrictToCurrentUser = true;
    offset = 0;
    take = 10;
}
const state = new State();

const populateCards = async function () {
    if (state.selectedTab === MyContributeTabEnum.Draft ||
        state.selectedTab === MyContributeTabEnum.Published ||
        state.selectedTab === MyContributeTabEnum.Unpublished ||
        state.selectedTab === MyContributeTabEnum.ActionRequired) {

        if (state.offset === 0) {
            state.cards = [];
        }

        // adding a timestamp as a query string param to bust cache on IE11
        const url = `/api/card/GetContributionCards/${state.selectedCatalogueId}/${state.selectedTab}/${state.restrictToCurrentUser}/${state.offset}/${state.take}?timestamp=${new Date().getTime()}`;
        await AxiosWrapper.axios.get(url)
            .then(response => {
                if (state.offset === 0) {
                    state.cards = response.data;
                } else {
                    state.cards = state.cards.concat(response.data);
                }
                state.cardsLoaded = true;
            })
            .catch(e => {
                console.log(e);
            });
    }
}
const populateCardTotals = async function () {
    state.cardsLoaded = false;
    state.cardTotalsLoaded = false;
    const url = `/api/card/GetMyContributionTotals/${state.selectedCatalogueId}?timestamp=${new Date().getTime()}`;
    await AxiosWrapper.axios.get(url)
        .then(response => {
            state.actionRequiredCount = response.data.actionRequiredCount;
            state.draftCount = response.data.draftCount;
            state.publishedCount = response.data.publishedCount;
            state.unpublishedCount = response.data.unpublishedCount;
            state.userActionRequiredCount = response.data.userActionRequiredCount;
            state.userDraftCount = response.data.userDraftCount;
            state.userPublishedCount = response.data.userPublishedCount;
            state.userUnpublishedCount = response.data.userUnpublishedCount;
            state.cardTotalsLoaded = true;
            state.offset = 0;
            if (state.selectedTab === MyContributeTabEnum.Unspecified && state.actionRequiredCount > 0) {
                state.selectedTab = MyContributeTabEnum.ActionRequired;
            }
            else if (state.selectedTab === MyContributeTabEnum.Unspecified && state.selectedCatalogueId == 1) {
                state.selectedTab = MyContributeTabEnum.Draft;
            }
            else if (state.selectedTab === MyContributeTabEnum.Unspecified) {
                state.selectedTab = MyContributeTabEnum.AllContent;
            }
            populateCards();
        })
        .catch(e => {
            console.log(e);
        });
}
const mutations = {
    setReadOnly(state: State, payload: boolean) {
        state.isReadonly = payload;
    },
    setCardsLoaded(state: State, payload: boolean) {
        state.cardsLoaded = payload;
    },
    setCatalogue(state: State, payload: number) {
        state.selectedCatalogue = state.userCatalogues.find(c => c.nodeId == payload);
        state.selectedCatalogueId = payload;
        state.cards = [];
        populateCardTotals();
    },
    setCatalogueByUrl(state: State, payload: string) {
        state.initialCatalogueUrl = payload;
    },
    setSelectedTab(state: State, payload: MyContributeTabEnum) {
        state.selectedTab = payload;
        state.offset = 0;
        if (state.cardTotalsLoaded) {
            populateCards();
        }
    },
    setRestrictToCurrentUser(state: State, payload: boolean) {
        state.restrictToCurrentUser = payload;
        state.offset = 0;
        populateCards();
    },
    setCatalogues(state: State, payload: any) {
        state.userCatalogues = payload;
        if (state.initialCatalogueUrl !== '') {
            const initialCatalogue = state.userCatalogues.find(c => c.url == state.initialCatalogueUrl);
            if (initialCatalogue !== undefined) {
                state.selectedCatalogue = initialCatalogue;
                state.selectedCatalogueId = initialCatalogue.nodeId;
            }
        }
        if (state.selectedCatalogue === null) {
            state.selectedCatalogue = state.userCatalogues[0];
            state.selectedCatalogueId = state.selectedCatalogue.nodeId;
        }
    },
    getNextPage(state: State) {
        state.offset += state.take;
        populateCards();
    }
} as MutationTree<State>;

const actions = <ActionTree<State, any>>{
    populateCatalogues(context) {
        resourceData.getCataloguesForUser()
            .then(response => {
                context.commit("setCatalogues", response);
                populateCardTotals();

                if (context.state.selectedCatalogue.nodeId > 1) {
                    context.dispatch('contentStructureState/populateCatalogue', context.state.selectedCatalogue.id, { root: true });
                }
            });
    },
    async refreshCardData() {
        // Needed to refresh the tab card totals on My Contributions screen after deleting a failed resource on the All Content tab (content structure treeview).
        populateCardTotals();
    },
    async deleteResourceVersion(context: ActionContext<State, State>, payload: { resource: MyContributionsCardModel }) {
        if (payload.resource.versionStatusEnum == VersionStatus.DRAFT || payload.resource.versionStatusEnum == VersionStatus.FAILED) {
            resourceData.deleteResourceVersion(payload.resource.draftResourceVersionId).then(async response => {
                await populateCardTotals();
            });
        }
    },
};

const getters = <GetterTree<State, any>>{};

export const myContributionsState = {
    namespaced: true,
    state,
    mutations,
    actions,
    getters
}

export default new Vuex.Store({
    modules: {
        myContributionsState,
        contentStructureState
    }
});

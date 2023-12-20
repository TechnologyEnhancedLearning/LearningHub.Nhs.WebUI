import Vue from 'vue';
import Vuex, { Store, Module } from 'vuex';
import { contentStructureState } from '../content-structure-admin/contentStructureState';

Vue.use(Vuex); 

/* NOTE: The only part of the catalogue screens that use vuex is the content-structure-admin component, 
 * which needs the contentStructureState to be available in its own module. */

export default new Vuex.Store({
    modules: {
        contentStructureState
    }
});

import Vue from 'vue';
import Vuelidate from 'vuelidate';
import { ContributeResourceRouter } from './contributeResourceRouter';
import store from '../contribute/contributeState';

Vue.use(Vuelidate as any);

new Vue({
    el: '#contributecontainer',
    router: ContributeResourceRouter,
    store,
    created() {
        this.$store.commit('populateContributeAVResourceFlag'); 
        this.$store.commit('populateAVUnavailableView'); 
        this.$store.commit('populateMKPlayerLicenceKey');
    }
});

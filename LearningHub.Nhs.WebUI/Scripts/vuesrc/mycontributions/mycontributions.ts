import Vue from 'vue';
import router from './mycontributions-router';
import Vuelidate from "vuelidate";
import MyContributions from './mycontributions.vue';
import GridCardComponent from './gridcardcomponent.vue';
import GridComponent from './gridcomponent.vue';
import MyContributionsCardHeader from './mycontributionscardheader.vue';
import store from './mycontributionsState';
import 'regenerator-runtime/runtime';
import 'core-js/stable';

Vue.use(Vuelidate as any);

new Vue({
    el: '#myContributions',
    router,
    store,
    components: {
        MyContributions,
        GridCardComponent,
        GridComponent,
        MyContributionsCardHeader
    },
    computed: {
    },
    created() {
    }

});

import Vue from 'vue';
import Vuelidate from 'vuelidate';
import { ContributeResourceRouter } from './contributeResourceRouter';

Vue.use(Vuelidate as any);

new Vue({
    el: '#contributecontainer',
    router: ContributeResourceRouter,
});

import Vue from 'vue';

import ManageCatalogue from './managecatalogue.vue';
import CatalogueAccessRequest from './catalogueaccessrequest.vue';
import { catalogueData } from '../data/catalogue';
import router from './catalogue-router';
import store from './catalogueState';

//Vue.use(Vuelidate);

new Vue({
    el: '#cataloguecontainer',
    router,
    store,
    components: {
        ManageCatalogue,
        CatalogueAccessRequest
    },
    data() {
        return {
        };
    },
    async created() {       
    },
    methods: {       
    },
    watch: {
        '$route'(to, from) {            
        }
    }
});


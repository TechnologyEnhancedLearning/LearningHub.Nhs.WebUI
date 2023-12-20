import Vue from 'vue';

import CataloguesList from './catalogueslist.vue';
import { catalogueData } from '../data/catalogue';
//Vue.use(Vuelidate);

new Vue({
    el: '#cataloguesList',      
    components: {        
        CataloguesList,
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


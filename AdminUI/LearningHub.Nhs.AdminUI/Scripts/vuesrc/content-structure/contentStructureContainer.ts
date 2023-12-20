import Vue from 'vue';
import router from './contentStructureRouter';
import store from './contentStructureState';

new Vue({
    el: '#contentStructureContainer',
    router,
    store,
    created() {
        if (this.$route.params.id) {
            this.$store.dispatch('contentStructureState/populateCatalogue', this.$route.params.id);
            //this.$store.commit('populateHierarchyEdit', this.$route.params.id);
        }
    }
});
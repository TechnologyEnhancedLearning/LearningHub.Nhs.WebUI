import Vue from 'vue';
import router from './contribute-router';
import Vuelidate from "vuelidate";
import Content from './Content.vue';
import About from './About.vue';
import Legal from './Legal.vue';
import Summary from './Summary.vue';
import store from './contributeState';
import 'regenerator-runtime/runtime';
import 'core-js/stable';

Vue.use(Vuelidate as any);

new Vue({
    el: '#contributecontainer',
    router,
    store,
    components: {
        Content,
        About,
        Legal,
        Summary
    },
    created() {
        this.$store.commit('populateContributeSettings');
        if (this.$route.params.rvId) {
            this.$store.commit('populateResource', this.$route.params.rvId);
        }
        if (this.$route.name === "ContributeNewCatalogueResource") {
            if (this.$route.params.catId) {
                this.$store.commit('setInitialCatalogue', this.$route.params.catId);
            }

            if (this.$route.params.nodeId) {
                this.$store.commit('setInitialNode', this.$route.params.nodeId);
            }

            this.$router.push({ name: 'ContributeAResource' });
        }
    }

});


import Vue from 'vue';
import router from './resource-router';
//import Vuelidate from "vuelidate";
import store from './resourceState';
import ResourceContent from './ResourceContent.vue';
import { resourceData } from '../data/resource';

//Vue.use(Vuelidate);

new Vue({
    el: '#resourcecontainer',
    router,
    store,
    components: {
        ResourceContent,
    },
    data() {
        return {
            resourceId: 0,
            resourceTitle: '',
            resourceLoading: false,
            menuName: ''
        };
    },
    async created() {
        this.menuName = this.menuItemName(this.$route.name);
        await this.loadResourceHeader(Number(this.$route.params.resId));
    },
    methods: {
        async loadResourceHeader(id: number) {
            this.resourceLoading = true;
            let header = await resourceData.getHeader(id);
            this.resourceId = header.id;
            this.resourceTitle = header.title;
            this.$store.commit('setHeader', header);
            this.resourceLoading = false;
        },
        menuItemName(routeName: string) {
            switch (routeName) {
                case 'History':
                    return 'Version history';
                case 'LocatedIn':
                    return 'Located in';
                default:
                    return routeName;
            }
        }     
    },
    watch: {
        '$route'(to, from) {
            this.menuName = this.menuItemName(to.name);
        }
    }
});


import Vue from 'vue';
import Router from 'vue-router';
import MyContributions from './mycontributions.vue';

Vue.use(Router);

const router = new Router({
    mode: 'history',
    routes: [
        {
            path: '/',
            redirect: '/my-contributions'
        },

        {
            path: '/my-contributions/:status/:catalogueUrl/:nodeId(\\d+)',
            name: 'MyContributionsStatusCatalogueNode',
            component: MyContributions
        },

        // status + catalogue
        {
            path: '/my-contributions/:status/:catalogueUrl',
            name: 'MyContributionsStatusCatalogue',
            component: MyContributions
        },

        // ONLY status
        {
            path: '/my-contributions/:status',
            name: 'MyContributionsStatus',
            component: MyContributions
        },

        // fallback → only `/my-contributions`
        {
            path: '/my-contributions',
            name: 'MyContributions',
            component: MyContributions
        },

        {
            path: '*',
            redirect: '/my-contributions'
        }
    ]
});

export default router;

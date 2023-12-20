import Vue from 'vue';
import Router from 'vue-router';
import contentStructure from './contentStructure.vue';

Vue.use(Router);

var router = new Router({
    mode: 'history',
    base: '/Catalogue/ContentStructure',
    routes: [
        {
            path: '/:id',
            name: 'ContentStructure',
            component: contentStructure,
            meta: {
                title: 'Content structure'
            },
            props: {
                showFoldersOnly: true,
                readOnly: false
            }
        },
    ]
});

// This callback runs before every route change, including on page load.
router.beforeEach((to, from, next) => {
    // This goes through the matched routes from last to first, finding the closest route with a title.
    // eg. if we have /some/deep/nested/route and /some, /deep, and /nested have titles, nested's will be chosen.
    const nearestWithTitle = to.matched.slice().reverse().find(r => r.meta && r.meta.title);

    // If a route with a title was found, set the document (page) title to that value.
    if (nearestWithTitle) document.title = nearestWithTitle.meta.title;

    next();
});

export default router;

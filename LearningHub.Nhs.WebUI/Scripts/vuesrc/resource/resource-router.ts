import Vue from 'vue';
import Router from 'vue-router';
import ResourceContent from './ResourceContent.vue';

Vue.use(Router);

var router = new Router({
    mode: 'history',
    base: '/Resource',
    routes: [
        {
            path: '/:resId',
            redirect: '/:resId/Item'
        },
        {
            path: '/:resId/Item',
            name: 'Resource',
            component: ResourceContent,
            meta: {
                title: 'Resource details'
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

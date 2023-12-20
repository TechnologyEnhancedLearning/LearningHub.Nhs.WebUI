import Vue from 'vue';
import Router from 'vue-router';
import Content from './Content.vue';
import About from './About.vue';
import Legal from './Legal.vue';
import Summary from './Summary.vue';

Vue.use(Router);

const router = new Router({
    mode: 'history',
    base: '/Contribute',
    routes: [
        {
            path: '/',
            redirect: '/contribute-a-resource'
        },
        {
            path: '/contribute-a-resource/:rvId?',
            name: 'ContributeAResource',
            component: Content,
            meta: {
                title: 'Contribute a Resource'
            }
        },
        {
            path: '/contribute-new-catalogue-resource/:catId/:nodeId?',
            name: 'ContributeNewCatalogueResource',
            component: Content,
            meta: {
                title: 'Contribute a Resource'
            }
        },
        //{
        //    path: '/about-your-resource/:rvId?',
        //    name: 'AboutYourResource',
        //    component: About,
        //    meta: {
        //        title: 'About your Resource'
        //    }
        //},
        //{
        //    path: '/legal/:rvId?',
        //    name: 'Legal',
        //    component: Legal,
        //    meta: {
        //        title: 'Legal'
        //    }
        //},
        //{
        //    path: '/summary/:rvId?',
        //    name: 'Summary',
        //    component: Summary,
        //    meta: {
        //        title: 'Summary'
        //    }
        //},
        {
            path: '*',
            redirect: '/contribute-a-resource'
        }
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

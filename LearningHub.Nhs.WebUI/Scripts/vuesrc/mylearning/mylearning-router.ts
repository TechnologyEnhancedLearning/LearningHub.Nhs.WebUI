import Vue from 'vue';
import Router from 'vue-router';
import ActivityDetailed from './ActivityDetailed.vue';
import ActivityDetailedReport from './ActivityDetailedReport.vue';

Vue.use(Router);

var router = new Router({
    mode: 'history',
    base: '/MyLearning',
    routes: [
        {
            path: '/',
            redirect: '/activity'
        },
        {
            path: '/activity',
            name: 'ActivityDetailed',
            component: ActivityDetailed,
            meta: {
                title: 'Itemised activity'
            }
        },
        {
            path: '/activity-report',
            name: 'ActivityDetailedReport',
            component: ActivityDetailedReport,
            meta: {
                title: 'Activity report'
            }
        },
        {
            path: '*',
            redirect: '/activity'
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

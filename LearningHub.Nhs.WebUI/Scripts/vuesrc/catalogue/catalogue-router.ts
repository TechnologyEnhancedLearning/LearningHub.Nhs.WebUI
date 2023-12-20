import Vue from 'vue';
import Router from 'vue-router';
import ManageCatalogue from './managecatalogue.vue';
import CatalogueAccessRequests from './catalogueaccessrequests.vue';
import CatalogueAccessRequest from './catalogueaccessrequest.vue';

Vue.use(Router);

var router = new Router({
    mode: 'history',
    routes: [
        {
            path: '/Catalogue/Manage/:reference',
            name: 'ManageCatalogue',
            component: ManageCatalogue,
        },
        {
            path: '/Catalogue/Manage/:reference/AccessRequest/:accessRequestId',
            name: 'AccessRequest',
            component: CatalogueAccessRequest,
        }
    ]
});

//// This callback runs before every route change, including on page load.
//router.beforeEach((to, from, next) => {
//    // This goes through the matched routes from last to first, finding the closest route with a title.
//    // eg. if we have /some/deep/nested/route and /some, /deep, and /nested have titles, nested's will be chosen.
//    const nearestWithTitle = to.matched.slice().reverse().find(r => r.meta && r.meta.title);

//    // If a route with a title was found, set the document (page) title to that value.
//    if (nearestWithTitle) document.title = nearestWithTitle.meta.title;

//    next();
//});

export default router;
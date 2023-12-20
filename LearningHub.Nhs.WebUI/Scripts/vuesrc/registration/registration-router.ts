import Vue from 'vue';
import Router from 'vue-router';
import CreateAccount from './CreateAccount.vue';
import NewUserNotEligible from './NewUserNotEligible.vue';
import ExistingUserNotEligible from './ExistingUserNotEligible.vue';
import ExistingUserIsEligible from './ExistingUserIsEligible.vue';
import PersonalInformation from './PersonalInformation.vue';
import CurrentJobRole from './CurrentJobRole.vue';
import PlaceOfWork from './PlaceOfWork.vue';
import Complete from './Complete.vue';

Vue.use(Router);

const router = new Router({
    mode: 'history',
    base: '/Registration',
    routes: [
        {
            path: '/',
            redirect: '/create-an-account'
        },
        {
            path: '/create-an-account',
            name: 'CreateAccount',
            component: CreateAccount,
            meta: {
                title: 'Create an account'
            }
        },
        {
            path: '/new-user-not-eligible',
            name: 'NewUserNotEligible',
            component: NewUserNotEligible,
            meta: {
                title: 'Cannot create account'
            }
        },
        {
            path: '/existing-user-not-eligible',
            name: 'ExistingUserNotEligible',
            component: NewUserNotEligible,
            meta: {
                title: 'Not eligible'
            }
            //name: 'ExistingUserNotEligible',
            //component: ExistingUserNotEligible,
            //meta: {
            //    title: 'Existing account not eligible'
            //}
        },
        {
            path: '/existing-user-is-eligible',
            name: 'ExistingUserIsEligible',
            component: ExistingUserIsEligible,
            meta: {
                title: 'Existing account'
            }
        },
        {
            path: '/personal-information',
            name: 'PersonalInformation',
            component: PersonalInformation,
            meta: {
                title: 'Account details'
            }
        },
        {
            path: '/current-job-role',
            name: 'CurrentJobRole',
            component: CurrentJobRole,
            meta: {
                title: 'Job role'
            }
        },
        {
            path: '/place-of-work',
            name: 'PlaceOfWork',
            component: PlaceOfWork,
            meta: {
                title: 'Place of work'
            }
        },
        {
            path: '/complete',
            name: 'Complete',
            component: Complete,
            meta: {
                title: 'Account complete'
            }
        },
        {
            path: '*',
            redirect: '/create-an-account'
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

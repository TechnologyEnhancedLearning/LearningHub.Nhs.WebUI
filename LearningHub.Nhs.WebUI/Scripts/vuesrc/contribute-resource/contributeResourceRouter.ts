import Vue from 'vue';
import VueRouter from 'vue-router';
import Contribute from './Contribute.vue';
import SelectResource from './SelectResource.vue';
import CaseOrAssessmentPreview from "./CaseOrAssessmentPreview.vue";

Vue.use(VueRouter);

export const ContributeResourceRouter = new VueRouter({
    mode: 'history',
    base: '/contribute-resource',
    routes: [
        {
            path: '/select-resource',
            component: SelectResource
        },
        {
            path: '/edit/:id',
            component: Contribute,
            props: (route => ({ resourceVersionId: route.params.id })),
        },
        {
            path: '/preview/:id',
            component: CaseOrAssessmentPreview,
            props: (route => ({ resourceVersionId: route.params.id })),
        },
        {
            path: '*',
            beforeEnter: () => {
                // For any route that doesn't match, show the 404/AccessDenied page
                window.location.href = '/home/AccessDenied';
            }
        }
    ]
});

import Vue from 'vue';
import Router from 'vue-router';
import pageDetail from './pageDetail.vue';
import pageSectionTemplate from './pageSectionTemplate.vue';
import pageImageSection from './pageImageSection.vue';
import pageVideoSection from './pageVideoSection.vue';

Vue.use(Router);

const page = {
    template: "<router-view></router-view>"
}

const router = new Router({
    mode: 'history',
    base: '/cms/page',
    routes: [
        {
            path: '/:pageId',
            component: page,
            children: [
                {
                    path: '',
                    name: 'PageDetail',
                    component: pageDetail,
                    meta: {
                        title: 'Page Template'
                    }
                },
                {
                    path: 'add-section',
                    name: 'SelectATemplate',
                    component: pageSectionTemplate,
                    meta: {
                        title: 'Select a template'
                    }
                },
                {
                    path: 'update-image-section/:sectionId',
                    name: 'ImageSection',
                    component: pageImageSection,
                    meta: {
                        title: 'Image and text'
                    }
                },
                {
                    path: 'update-video-section/:sectionId',
                    name: 'VideoSection',
                    component: pageVideoSection,
                    meta: {
                        title: 'Video and text'
                    }
                }
            ]
        }
    ]
});

export default router;
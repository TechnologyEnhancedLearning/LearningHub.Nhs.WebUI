<template>
    <div>
        <page-header :page="page"
                     :publishCompleted="publishCompleted"
                     @discard="onDiscard()"
                     @toggleSection="onToggleSection"
                     @toggleToolBar="onToggleToolBar"
                     @publish="onPublish()"
                     @publishReset="onPublishReset()">
        </page-header>
        <div v-for="(pageSection, index) in page.pageSections" :key="pageSection.Id" class="position-relative">

            <page-section-toolbar v-show="showToolbar"
                                  :page-section="pageSection"
                                  :itemIndex="index"
                                  :totalItems="page.pageSections.length"
                                  @moveUp="onMoveUp"
                                  @moveDown="onMoveDown"
                                  @clone="onClone"
                                  @hide="onHide"
                                  @unHide="onUnHide"
                                  @edit="onEdit"
                                  @addNew="onAddNew"
                                  @delete="onDelete">
            </page-section-toolbar>

            <cms-page-row :section="pageSection.pageSectionDetail"
                        :section-template-type="pageSection.sectionTemplateType"
                        :is-hidden="pageSection.isHidden">
            </cms-page-row>
        </div>
    </div>
</template>
<script lang="ts">
    import Vue from 'vue';
    import { contentData } from '../data/content';
    import pageHeader from './pageHeader.vue';
    import cmsPageRow from './cmsPageRow.vue';
    import pageSectionToolbar from './pageSectionToolbar.vue';
    import { PageModel } from '../models/content/pageModel';
    import { DirectionType, SectionTemplateType } from '../models/content/pageSectionModel';
    import { SectionLayoutType } from '../models/content/pageSectionDetailModel';

    export default Vue.extend({
        components: {
            cmsPageRow,
            pageHeader,
            pageSectionToolbar
        },
        props: {
        },
        data() {
            return {
                SectionLayoutType: SectionLayoutType,
                SectionTemplateType: SectionTemplateType,
                page: new PageModel(),
                showHiddenSections: false,
                showToolbar: true,
                publishCompleted: false,
            }
        },
        beforeCreate: function () {
            $('body, .container-fluid').css('background-color', '#f0f4f5');
        },
        created() {
            this.loadPage(parseInt(this.$route.params.pageId));
        },
        methods: {
            loadPage(pageId: number) {
                if (this.showHiddenSections) {
                    contentData.getPageWithAllSections(pageId).then(response => {
                        this.page = response;
                    });
                } else {
                    contentData.getPage(pageId).then(response => {
                        this.page = response;
                    });
                }
            },
            onDiscard() {
                contentData.discardPageChanges(this.page.id).then(response => {
                    this.loadPage(this.page.id);
                });
            },
            onToggleSection() {
                this.showHiddenSections = !this.showHiddenSections;
                this.loadPage(this.page.id);
            },
            onToggleToolBar() {
                this.showToolbar = !this.showToolbar;
            },
            onPublish() {
                this.publishCompleted = false;
                contentData.publishPageChanges(this.page.id).then(response => {
                    this.loadPage(this.page.id);
                    this.publishCompleted = true;
                });
            },
            onPublishReset() {
                this.publishCompleted = false;
            },
            onMoveUp(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                contentData.changeOrder(this.page.id, pageSection.id, DirectionType.Up).then(response => {
                    this.loadPage(this.page.id);
                });
            },
            onMoveDown(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                contentData.changeOrder(this.page.id, pageSection.id, DirectionType.Down).then(response => {
                    this.loadPage(this.page.id);
                });
            },
            onClone(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                contentData.cloneSection(pageSection.id).then(response => {
                    this.loadPage(this.page.id);
                });
            },
            onHide(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                contentData.hideSection(pageSection.id).then(response => {
                    this.loadPage(this.page.id);
                });
            },
            onUnHide(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                contentData.unHideSection(pageSection.id).then(response => {
                    this.loadPage(this.page.id);
                });
            },
            onEdit(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                window.location.href = `/cms/page/${this.page.id}/update-${pageSection.sectionTemplateType == SectionTemplateType.Video ? "video" : "image"}-section/${pageSection.id}`;
            },
            onAddNew(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                window.location.href = `/cms/page/${this.page.id}/add-section?position=${pageSection.position}`;
            },
            onDelete(itemIndex: number) {
                let pageSection = this.page.pageSections[itemIndex];
                contentData.deleteSection(pageSection.id).then(response => {
                    this.loadPage(this.page.id);
                });
            },
        },
    });
</script>

<style type="text/css">
    .cms-page .header-row {
        font-size: 32px;
        font-weight: bold;
    }

    .cms-page h2 {
        font-size: 32px;
    }

    .cms-page p {
        margin: 2rem 0;
    }

    .cms-page img {
        max-width: 100%;
        max-height: 100%;
    }

    .cms-page .desc-position-left {
        padding-left: 3rem;
    }

    .cms-page .col-order-first {
        padding-right: 0;
    }

    @media (max-width: 992px) {
        .cms-page {
            text-align: center !important;
        }

        .cms-page p {
            font-size: 16px;
        }

        .cms-page img {
            max-width: 60%;
        }

        .cms-page .desc-position-left, .cms-page .desc-position-right {
            padding: 0 3rem;
        }
    }

    @media (min-width: 992px) {
        .col-order-last {
            order: 13;
        }

        .col-order-first {
            order: -1;
        }
    }

    @media (max-width: 768px) {
        .cms-page h2 {
            font-size: 24px;
        }

        .cms-page img {
            max-width: 100%;
        }

        .cms-page .desc-position-left, .cms-page .desc-position-right {
            padding: 0 2rem;
        }
    }
</style>
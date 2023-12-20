<template>
    <div class="py-5">
        <a :href="[`/cms/page/${$route.params.pageId}`]">
            <i class="fa-solid fa-chevron-left">&nbsp;</i>
            Landing page edit view
        </a>

        <div class="pt-5 pb-4">
            <h1>Select a template</h1>
            Before you can continue, choose a row template
        </div>

        <div class="d-flex align-items-center border-bottom py-5">
            <div class="pr-5 position-relative">
                <img src="/images/image-template.svg" alt="Image and text" />
                <div :class="[`${selected == 'image' ? 'template-svg-selected' : 'd-none'}`]"></div>
            </div>
            <div class="pr-5 w-50">
                <p><strong>Image and text</strong></p>
                <p>If you have an image, for example a .jpg or .png and accompanying text.</p>
            </div>
            <div>
                <i v-if="selected == 'image'" class="fas fa-check-circle fa-3x nhs-green-color" />
                <button v-else class="btn btn-nhs-common btn-outline-primary" type="button" @click="select('image')">Select</button>
            </div>
        </div>

        <div class="d-flex align-items-center border-bottom py-5">
            <div class="pr-5 position-relative">
                <img src="/images/video-template.svg" alt="Video and text" />
                <div :class="[`${selected == 'video' ? 'template-svg-selected' : 'd-none'}`]"></div>
            </div>
            <div class="pr-5 w-50">
                <p><strong>Video and text</strong></p>
                <p>If you have a video, for example .mp4 or .avi and accompanying text.</p>
            </div>
            <div>
                <i v-if="selected == 'video'" class="fas fa-check-circle fa-3x nhs-green-color" />
                <button v-else class="btn btn-nhs-common btn-outline-primary" type="button" @click="select('video')">Select</button>
            </div>
        </div>

        <button :class="[`my-5 btn btn-nhs-common ${selected === undefined ? 'btn-secondary' : 'btn-success'}`]"
                :disabled="selected == undefined" @click="redirect" type="button">Next</button>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import router from './contentRouter';
    import { PageSectionModel, SectionTemplateType } from '../models/content/pageSectionModel';
    import { PageSectionDetailModel } from '../models/content/pageSectionDetailModel';
    import { contentData } from '../data/content';

    export default Vue.extend({
        data() {
            return {
                selected: undefined as string
            }
        },
        methods: {
            select(type: string) {
                this.selected = type;
            },
            redirect() {
                let pageSection = new PageSectionModel({
                    pageId: parseInt(this.$route.params.pageId),
                    position: parseInt(this.$route.query.position as string || '1'),
                    sectionTemplateType: this.selected == 'video' ? SectionTemplateType.Video : SectionTemplateType.Image,
                    pageSectionDetail: new PageSectionDetailModel()
                });

                contentData.createPageSection(pageSection).then(pageSectionId => {
                    router.push(`update-${this.selected}-section/${pageSectionId}`);
                });
            }
        },
    });
</script>
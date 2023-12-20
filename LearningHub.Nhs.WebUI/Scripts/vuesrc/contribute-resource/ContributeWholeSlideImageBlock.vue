<template>
    <div class="contribute-whole-slide-image-block">
        <div class="px-20">
            <div v-for="wholeSlideImageBlockItem in wholeSlideImageBlock.wholeSlideImageBlockItems"
                 :key="wholeSlideImageBlockItem.wholeSlideImage.wholeSlideImageRef">
                <ContributeWholeSlideImage :resourceType="resourceType"
                                           :wholeSlideImageItem="wholeSlideImageBlockItem"
                                           :enableUp="!wholeSlideImageBlock.isFirstSlide(wholeSlideImageBlockItem)"
                                           :enableDown="!wholeSlideImageBlock.isLastSlide(wholeSlideImageBlockItem)"
                                           @up="wholeSlideImageBlock.moveSlideUp(wholeSlideImageBlockItem)"
                                           @down="wholeSlideImageBlock.moveSlideDown(wholeSlideImageBlockItem)"
                                           @delete="wholeSlideImageBlock.deleteSlide(wholeSlideImageBlockItem)"
                                           @annotateWholeSlideImage="showSlideWithAnnotations"/>
            </div>
        </div>
        <div class="contribute-whole-slide-image-block-add-slide">
            <Button class="contribute-whole-slide-image-block-add-slide--button"
                    size="small"
                    @click="addSlide">
                + Add {{ wholeSlideImageBlock.wholeSlideImageBlockItems.length > 0 ? 'more' : '' }} slides
            </Button>
            <div class="contribute-whole-slide-image-block-add-slide--info">
                You can upload {{ allowedFileExtensionsFormatted }} files from your computer or other storage drive you
                are connected to. Please upload MIRAX (.mrxs) as a zip file,
                Maximum file size {{ maxFileSizeFormatted }}
            </div>
        </div>
    </div>
</template>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Button from '../globalcomponents/Button.vue';
    import EditSaveFieldWithCharacterCount from '../globalcomponents/EditSaveFieldWithCharacterCount.vue';
    import { WholeSlideImageBlockModel } from '../models/contribute-resource/blocks/wholeSlideImageBlockModel';
    import {
        FileUploadType,
        getAllowedFileExtensionsInAcceptFormat,
        getFileExtensionAllowedListFormatted,
        getMaxFileSizeFormatted,
    } from '../helpers/fileUpload';
    import ContributeWholeSlideImage from './ContributeWholeSlideImage.vue';
    import { WholeSlideImageModel } from "../models/contribute-resource/blocks/wholeSlideImageModel";
    import { ResourceType } from "../constants";

    export default Vue.extend({
        props: {
            wholeSlideImageBlock: { type: Object } as PropOptions<WholeSlideImageBlockModel>,
            resourceType: { type: Number } as PropOptions<ResourceType>,
        },
        components: {
            Button,
            ContributeWholeSlideImage,
            EditSaveFieldWithCharacterCount,
        },
        data() {
            return {
                allowedFileExtensionsFormatted: getFileExtensionAllowedListFormatted(FileUploadType.WholeSlideImage),
                allowedFileExtensionsInAcceptFormat: getAllowedFileExtensionsInAcceptFormat(FileUploadType.WholeSlideImage),
                maxFileSizeFormatted: getMaxFileSizeFormatted(),
            };
        },
        methods: {
            addSlide() {
                this.wholeSlideImageBlock.addSlide(undefined);
            },
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow);
            },
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-whole-slide-image-block {
        background-color: $nhsuk-white;
    }

    .contribute-whole-slide-image-block-add-slide {
        display: flex;
        justify-content: flex-start;
        padding: 25px;
        border-top: 1px solid $nhsuk-grey-light;

        &--info {
            max-width: 700px;
            font-size: 16px;
            margin-left: 24px;
            margin-bottom: 0;
        }

        & > &--button {
            min-width: fit-content;
            height: 48px;
            margin: auto 0;
        }
    }
</style>

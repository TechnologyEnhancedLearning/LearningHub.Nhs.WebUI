<template>
    <div>
        <div class="d-flex flex-row flex-wrap">
            <div class="col-8 p-0">
                <div class="contribute-case-component lh-padding-fluid">
                    <div class="lh-container-xl py-15">
                        <ContributeWholeSlideImage :resourceType="resourceType"
                                                   :wholeSlideImageItem="wholeSlideImageBlockItem"
                                                   :imageZone="true"
                                                   @annotateWholeSlideImage="showSlideWithAnnotations"
                                                   @delete="imageZoneDelete"/>
                    </div>
                </div>
            </div>
            <div class="col-4 p-0 pt-15 description-text">
                <div v-if="!this.wholeSlideImageBlockItem.wholeSlideImage.file.fileId">
                    <p>
                        Please upload the Whole Slide Image for this question.
                    </p>
                    <p>
                        You can upload {{ allowedFileExtensionsFormatted }} files from your computer or other storage
                        drive you are connected to.
                        Maximum file size {{ maxFileSizeFormatted }}.
                    </p>
                </div>
                <div v-else>
                    <p>
                        Once processed, you must add between two and ten annotations to this image.
                    </p>
                    <p>
                        These will be the answer options that the learner will choose from to answer the question.
                    </p>
                    <p>
                        At least one answer option must be correct for the question to be valid.
                    </p>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import _ from 'lodash';
    import { BlockTypeEnum } from '../../../models/contribute-resource/blocks/blockTypeEnum';
    import { QuestionBlockModel } from '../../../models/contribute-resource/blocks/questionBlockModel';
    import { WholeSlideImageModel } from "../../../models/contribute-resource/blocks/wholeSlideImageModel";
    import { ResourceType } from "../../../constants";
    import {
        FileUploadType, getFileExtensionAllowedListFormatted, getMaxFileSizeFormatted,
    } from "../../../helpers/fileUpload";
    import ContributeWholeSlideImage from "../../ContributeWholeSlideImage.vue";
    import { WholeSlideImageBlockModel } from "../../../models/contribute-resource/blocks/wholeSlideImageBlockModel";
    import { WholeSlideImageBlockItemModel } from "../../../models/contribute-resource/blocks/wholeSlideImageBlockItemModel";

    export default Vue.extend({
        props: {
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>,
            resourceType: { type: Number } as PropOptions<ResourceType>,
        },

        components: {
            ContributeWholeSlideImage,
        },

        data() {
            return {
                wholeSlideImageBlock: null as WholeSlideImageBlockModel,
                wholeSlideImageBlockItem: null as WholeSlideImageBlockItemModel,
                maxFileSizeFormatted: getMaxFileSizeFormatted(),
                allowedFileExtensionsFormatted: getFileExtensionAllowedListFormatted(FileUploadType.WholeSlideImage),
            };
        },
        
        watch: {
            questionBlock: {
                immediate: true,
                handler()
                {
                    this.imageZoneCreate();
                }
            },    
        },
        
        async created() {
            this.updatedAnswer = _.debounce(this.updatedAnswerDebounced, 100);
        },

        methods: {
            imageZoneDelete() {
                this.wholeSlideImageBlock.deleteSlide(this.wholeSlideImageBlockItem);
                this.wholeSlideImageBlockItem = this.wholeSlideImageBlock.addSlide();
                this.questionBlock.answers = [];
                this.updatedAnswer();
            },
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, this.questionBlock);
            },
            imageZoneCreate() {
                const wholeSlideImageBlockModel = this.questionBlock.questionBlockCollection.blocks.find(b => b.blockType === BlockTypeEnum.WholeSlideImage);
                if (!wholeSlideImageBlockModel) {
                    this.wholeSlideImageBlock = this.questionBlock.questionBlockCollection.addBlock(BlockTypeEnum.WholeSlideImage).wholeSlideImageBlock;
                    this.wholeSlideImageBlockItem = this.wholeSlideImageBlock.addSlide();
                } else {
                    this.wholeSlideImageBlock = wholeSlideImageBlockModel.wholeSlideImageBlock;
                    this.wholeSlideImageBlockItem = this.wholeSlideImageBlock.wholeSlideImageBlockItems[0];
                }
            },
            updatedAnswerDebounced() {
                this.$emit("updatedAnswer");
            },
            updatedAnswer() {
                // replaced with debounced in created()
            },
        },
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .description-text {
        color: $nhsuk-grey;
    }
</style>
<template>
    <div>
        <ContributePagination :pageNumber="page"
                              :pageCount="pageCount"
                              :pageIsReady="this.blockCollection.isPageReady(page)"
                              @nextPage="nextPage"
                              @previousPage="previousPage"
                              @newPage="newPage"></ContributePagination>
        <ContributeAssessmentSettings v-if="resourceType === ResourceType.ASSESSMENT"
                                      :blockCollection="blockCollection"
                                      :firstTimeOpen="firstTimeOpen"/>
        <div class="lh-padding-fluid"
             v-if="duplicatingBlocks">
            <div class="lh-container-xl px-20 white-bg py-20 my-2">
                Autosave is disabled when copying content. Any changes made during this process will not be saved or
                copied.
                <div class="d-flex mt-3">
                    <Button color="blue"
                            class="mr-4"
                            :disabled="blocksToDuplicate.length === 0"
                            @click="showCopyContentModal = true">
                        Copy content
                    </Button>
                    <Button color="blue"
                            @click="exitCopyMode">
                        Cancel
                    </Button>
                </div>
            </div>
        </div>
        <ContributeCaseOrAssessmentContent :page="page"
                                           :blockCollection.sync="blockCollection"
                                           :resourceType="resourceType"
                                           :blocksToDuplicate="blocksToDuplicate"
                                           :duplicatingBlocks="duplicatingBlocks"
                                           @annotateWholeSlideImage="showSlideWithAnnotations"
                                           @duplicateBlock="duplicateBlock"></ContributeCaseOrAssessmentContent>
        <ContributeCaseOrAssessmentQuestions :page="page"
                                             :duplicatingBlocks="duplicatingBlocks"
                                             :blocksToDuplicate="blocksToDuplicate"
                                             :blockCollection.sync="blockCollection"
                                             :resourceType="resourceType"
                                             @annotateWholeSlideImage="showSlideWithAnnotations"
                                             @duplicateBlock="duplicateBlock"></ContributeCaseOrAssessmentQuestions>
        <CopyContentModal v-if="showCopyContentModal"
                          :blocksToBeCopied="blocksToDuplicate.length"
                          :resourceType="resourceType"
                          @cancel="showCopyContentModal = false"
                          @duplicate="duplicateBlocks"/>
        <DeletePageModal
            v-if="showDeleteModal"
            @cancel="showDeleteModal = false"
            @confirm="deletePage"/>
        <div class="lh-padding-fluid mb-5"
             v-if="pageCount > 1">
            <div class="lh-container-xl text-right delete-text">
                <a href="#"
                   @click.prevent="attemptDeletePage">Delete this page <i class="far fa-trash-can-alt"></i></a>
            </div>
        </div>
    </div>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import ContributeCaseOrAssessmentContent from './ContributeCaseOrAssessmentContent.vue';
    import ContributeCaseOrAssessmentQuestions from "./ContributeCaseOrAssessmentQuestions.vue";
    import ContributePagination from "./ContributePagination.vue";
    import DeletePageModal from './DeletePageModal.vue';
    import ContributeAssessmentSettings from "./ContributeAssessmentSettings.vue";
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { WholeSlideImageModel } from "../models/contribute-resource/blocks/wholeSlideImageModel";
    import { BlockCollectionModel } from '../models/contribute-resource/blocks/blockCollectionModel';
    import { ResourceType } from '../constants';
    import { QuestionBlockModel } from "../models/contribute-resource/blocks/questionBlockModel";
    import Button from "../globalcomponents/Button.vue";
    import { resourceData } from "../data/resource";
    import { AssessmentTypeEnum } from "../models/contribute-resource/blocks/assessments/assessmentTypeEnum";
    import CopyContentModal from "./CopyContentModal.vue";

    export default Vue.extend({
        props: {
            blockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            resourceType: { type: Number } as PropOptions<ResourceType>,
            firstTimeOpen: Boolean,
            resourceId: Number,
            duplicatingBlocks: Boolean,
        },
        components: {
            CopyContentModal,
            Button,
            ContributeCaseOrAssessmentContent,
            ContributeCaseOrAssessmentQuestions,
            ContributePagination,
            DeletePageModal,
            ContributeAssessmentSettings,
        },
        data() {
            return {
                page: 0,
                ResourceType,
                showDeleteModal: false,
                blocksToDuplicate: [] as number[],
                savePaused: false,
                showCopyContentModal: false,
            };
        },
        computed: {
            pageCount(): number {
                return this.blockCollection.getPages().length;
            },
            pageHasContent(): boolean {
                return this.blockCollection.getBlocksByPage(this.page)?.length > 0;
            }
        },
        methods: {
            nextPage() {
                this.page += 1;
            },

            previousPage() {
                this.page -= 1;
            },

            newPage() {
                this.blockCollection.addBlock(BlockTypeEnum.PageBreak, this.page);
                this.nextPage();
            },
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel, imageZone: Boolean, questionBlock: QuestionBlockModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, imageZone, questionBlock);
            },
            attemptDeletePage() {
                if (this.pageHasContent) {
                    this.showDeleteModal = true;
                } else {
                    this.deletePage();
                }
            },
            deletePage() {
                const previousPageCount = this.pageCount;
                this.blockCollection.deletePage(this.page);
                // If deleting the very last page, go to the previous page as a page with that number no longer exists
                if (this.page + 1 === previousPageCount) {
                    this.previousPage();
                }
                this.showDeleteModal = false;
            },
            duplicateBlock(blockId: number) {
                const index = this.blocksToDuplicate.findIndex((id) => id === blockId);
                if (index === -1) {
                    this.blocksToDuplicate.push(blockId);
                } else {
                    this.blocksToDuplicate.splice(index, 1);
                }
            },
            exitCopyMode() {
                this.$emit('cancelDuplicatingBlocks');
                this.blocksToDuplicate = [];
            },
            async duplicateBlocks(type: AssessmentTypeEnum, destinationResourceId?: number) {
                const childResourceVersionId = await resourceData.duplicateBlocks(this.resourceId, this.blocksToDuplicate, type, destinationResourceId)
                    .then((createdId) => {
                        return createdId;
                    })
                    .catch(() => {
                    });

                if (childResourceVersionId > 0) {
                    window.location.pathname = './contribute-resource/edit/' + childResourceVersionId;
                }
            }
        },
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .delete-text {
        a {
            font-size: 16px;
            color: $nhsuk-red;
        }

        a:hover {
            color: $nhsuk-red-hover;
        }
    }

    .white-bg {
        background: $nhsuk-white;
    }
</style>
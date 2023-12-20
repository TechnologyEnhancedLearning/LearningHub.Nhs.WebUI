<template>
    <div id="annotation-edit-start"
         class="annotation-edit-fullscreen">
        <div class="annotation-edit-fs-return">
            <div class="mr-10 my-0 annotation-action-bar-saving-wrapper">
                <ContributeSavingBar :saving-state="savingState"/>
            </div>
            <div>
                <Button size="medium"
                        @click="$emit('annotateWholeSlideImage')">
                    Back to draft {{ resourceName }}
                </Button>
            </div>
        </div>
        <div class="annotation-edit-fs-functions">
            <div class="annotation-edit-fs-functions-change-title">
                <EditSaveFieldWithCharacterCount v-model="wholeSlideImage.title"
                                                 addEditLabel="title"
                                                 :characterLimit="60"
                                                 :isH4="true"/>
            </div>
            <div class="annotation-edit-fs-functions-other">
                <div class="edit-hide-controls">
                    <LinkTextAndIcon @click="toggleVisibleControls">
                        <i class="fa-solid fa-plus"></i> {{ controlsVisible ? 'Hide Controls' : 'Show Controls' }}
                    </LinkTextAndIcon>
                </div>
                <div class="edit-undo"
                     hidden>
                    <i class="fa-solid fa-plus"></i>Undo
                </div>
                <div class="edit-redo"
                     hidden>
                    <i class="fa-solid fa-plus"></i>Redo
                </div>
            </div>
        </div>
        <div class="annotation-edit-fs-image">
            <SlideViewerWithAnnotations :whole-slide-image="wholeSlideImage"
                                        :controls-visible="controlsVisible"
                                        :is-editable="true"
                                        :image-zone="imageZone"
                                        :question-block="questionBlock"
                                        :answers="answers"
                                        :is-full-screen="false"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import SlideViewerWithAnnotations from '../../resource/blocks/SlideViewerWithAnnotations.vue';
    import EditSaveFieldWithCharacterCount from '../../globalcomponents/EditSaveFieldWithCharacterCount.vue';
    import { WholeSlideImageModel } from '../../models/contribute-resource/blocks/wholeSlideImageModel';
    import Button from '../../globalcomponents/Button.vue';
    import LinkTextAndIcon from '../../globalcomponents/LinkTextAndIcon.vue';
    import ContributeSavingBar from "../../contribute-resource/components/ContributeSavingBar.vue";
    import { ResourceType } from "../../constants";
    import { getDisplayNameForResourceType } from "../helpers/resourceHelper";
    import { QuestionBlockModel } from "../../models/contribute-resource/blocks/questionBlockModel";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";

    export default Vue.extend({
        name: "AnnotationFullPageEdit",
        components: {
            ContributeSavingBar,
            SlideViewerWithAnnotations,
            EditSaveFieldWithCharacterCount,
            Button,
            LinkTextAndIcon
        },
        props: {
            wholeSlideImage: { type: WholeSlideImageModel },
            savingState: String,
            resourceType: { type: Number } as PropOptions<ResourceType>,
            imageZone: Boolean,
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>,
        },
        data() {
            return {
                controlsVisible: true,
            };
        },
        computed: {
            answers(): AnswerModel[] {
                return this.questionBlock?.answers;
            },
            resourceName(): string {
                return getDisplayNameForResourceType(this.resourceType);
            },
        },
        methods: {
            toggleVisibleControls() {
                this.controlsVisible = !this.controlsVisible;
            }
        },
    });
</script>

<style lang="scss"
       scoped>
    @use "../../../../Styles/abstracts/all" as *;

    .annotation-edit-fullscreen {
        background-color: $nhsuk-white;

        .annotation-edit-fs-return {
            display: flex;
            padding: 20px;
            border-bottom: $nhsuk-grey-light 1px solid;
            justify-content: space-between;

            .annotation-action-bar-saving-wrapper {
                flex: 1 0 0;
                display: flex;
                align-items: center;
                max-width: 220px;
                justify-content: flex-end;

                @media screen and (min-width: 525px) {
                    justify-content: center;
                }
            }
        }

        .annotation-edit-fs-functions {
            display: flex;
            padding: 10px;
            justify-content: space-between;

            .annotation-edit-fs-functions-change-title {
                margin-right: 10px;
            }

            .annotation-edit-fs-functions-other {
                display: inline-flex;

                .edit-hide-controls {
                    margin: 10px;
                }

                .edit-undo {
                    color: $nhsuk-grey-light;
                    margin: 10px;
                }

                .edit-redo {
                    color: $nhsuk-grey-light;
                    margin: 10px;
                }
            }
        }

        .annotation-edit-fs-image {
            height: 800px;
        }
    }
</style>
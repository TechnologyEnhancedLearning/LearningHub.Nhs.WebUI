<template>
    <div class="slide-viewer-annotation-ui">
        <div class="annotation-ui-header">
            <strong>{{ imageZone ? "Answer options" : "Annotations" }}</strong>
        </div>
        <div class="annotation-content-and-footer">
            <div class="annotation-content overflow-auto"
                 v-if="listOfAnnotations.length > 0">
                <div v-for="(annotation) in this.listOfAnnotations">
                    <div class="annotation-content-list-item">
                        <AnnotationItem :label="annotation.label"
                                        :order="annotation.order"
                                        :answers="answers"
                                        :description="annotation.description"
                                        :placed="!(addingAnnotation && 
                                                        (listOfAnnotations.indexOf(annotation) === listOfAnnotations.length -1))"
                                        :currently-editing=" listOfAnnotations.indexOf(annotation) === indexOfEditingAnnotation"
                                        :currently-selected="listOfAnnotations.indexOf(annotation) === indexOfEditingAnnotation ||
                                                                   listOfAnnotations.indexOf(annotation) === indexOfSelectedAnnotation ||
                                                                   listOfAnnotations.indexOf(annotation) === indexOfDeletingAnnotation ||
                                                                   listOfAnnotations.indexOf(annotation) === indexOfPinEdited"
                                        :currently-viewing-details="listOfAnnotations.indexOf(annotation) === indexOfSelectedAnnotation &&
                                                                          indexOfSelectedAnnotation > -1"
                                        :pin-being-moved="  listOfAnnotations.indexOf(annotation) === indexOfPinEdited &&
                                                                  indexOfPinEdited > -1"
                                        :last-index-in-list="listOfAnnotations.length -1"
                                        :option-open-or-close="listOfAnnotations.indexOf(annotation) === indexOfOpenOptions"
                                        :is-editable="isEditable"
                                        :drawing-mode-enabled="drawingModeEnabled"
                                        :annotation-marks="annotation.imageAnnotationMarks"
                                        :annotation="annotation"
                                        :selected-mark-index="selectedMarkIndex"
                                        :feedbackVisible="feedbackVisible"
                                        @editAnnotationClicked="editAnnotation"
                                        @deleteAnnotationClicked="deleteAnnotation"
                                        @showDetails="showDetails"
                                        @movePin="movePin"
                                        @newAnnotationDone="$emit('newAnnotationDone')"
                                        @newAnnotationDiscard="$emit('newAnnotationDiscard')"
                                        @pinDoneMoving="$emit('pinDoneMoving')"
                                        @optionsClicked="setOpenOptionIndex"
                                        @moveAnnotationUp="moveAnnotationUp"
                                        @moveAnnotationDown="moveAnnotationDown"
                                        @stopDrawing="$emit('stopDrawing')"
                                        @startDrawing="startDrawing"
                                        @annotationMarkDeleted="$emit('annotationMarkDeleted')"
                                        @annotationColourChanged="$emit('annotationColourChanged')"
                                        @selectAMark="selectAMark"/>
                    </div>
                </div>
            </div>
            <div class="annotation-ui-footer"
                 v-if="isEditable && (!imageZone || !questionBlock.isFull())">
                <div v-if="!addingAnnotation">
                    <Button @click="addNewAnnotation"
                            size="small"
                            class="nhsuk-u-font-size-16"
                            color="green">
                        + Add an {{ answerOptionOrAnnotation }}
                    </Button>
                </div>
                <div v-if="addingAnnotation">
                    <Button disabled
                            size="small"
                            class="nhsuk-u-font-size-16 annotation-ui-add-annotation-disabled">
                        + Add an {{ answerOptionOrAnnotation }}
                    </Button>
                    <div class="py-2">
                        An {{ answerOptionOrAnnotation }} is being added
                    </div>
                </div>
            </div>
            <div class="annotation-ui-footer"
                 v-if="isEditable && !addingAnnotation && questionBlock && questionBlock.isFull()">
                <Button disabled
                        size="medium"
                        class="annotation-ui-add-annotation-disabled">
                    + Add an {{ answerOptionOrAnnotation }}
                </Button>
                <div class="p-1 justify-content-start">
                    {{ questionBlock.getMaximumReachedMessage() }}
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import AnnotationItem from '../../resource/blocks/AnnotationItem.vue';
    import Button from '../../globalcomponents/Button.vue';
    import { ImageAnnotation } from "../../models/contribute-resource/blocks/annotations/imageAnnotationModel";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import { QuestionBlockModel } from "../../models/contribute-resource/blocks/questionBlockModel";


    export default Vue.extend({
        components: { Button, AnnotationItem },
        props: {
            imageZone: Boolean,
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>,
            listOfAnnotations: { type: Array } as PropOptions<ImageAnnotation[]>,
            addingAnnotation: { type: Boolean, default: false },

            // TODO 11297, the number of indexes here will be reduced and simplified
            indexOfEditingAnnotation: Number,
            indexOfSelectedAnnotation: Number,
            indexOfDeletingAnnotation: Number,
            indexOfPinEdited: Number,
            isEditable: Boolean,
            drawingModeEnabled: Boolean,
            selectedMarkIndex: Number,
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            feedbackVisible: { type: Boolean, default: false } as PropOptions<boolean>,
        },
        data() {
            return {
                indexOfOpenOptions: -1,
                answerOptionOrAnnotation: this.imageZone ? "answer option" : "annotation",
            };
        },
        methods: {
            addNewAnnotation() {
                this.$emit('addAnnotationButtonPushed');
                this.indexOfOpenOptions = -1;
            },
            editAnnotation(index: number) {
                this.$emit('editAnnotationClicked', index);
                this.indexOfOpenOptions = -1;
            },
            deleteAnnotation(index: number) {
                this.$emit('deleteAnnotationClicked', index);
                this.indexOfOpenOptions = -1;
            },
            showDetails(index: number) {
                this.$emit('showAnnotationDetails', index);
                this.indexOfOpenOptions = -1;
            },
            movePin(index: number) {
                this.$emit('movePinMode', index);
                this.indexOfOpenOptions = -1;
            },
            setOpenOptionIndex(index: number) {
                if (this.indexOfOpenOptions === index) {
                    this.indexOfOpenOptions = -1;
                } else {
                    this.indexOfOpenOptions = index;
                }
            },
            moveAnnotationUp(index: number) {
                this.$emit('moveAnnotationUp', index);
                this.indexOfOpenOptions = -1;
            },
            moveAnnotationDown(index: number) {
                this.$emit('moveAnnotationDown', index);
                this.indexOfOpenOptions = -1;
            },
            startDrawing(index: number) {
                this.$emit('startDrawing', index);
            },
            selectAMark(annotationIndex: number, markIndex: number) {
                this.$emit('selectAMark', annotationIndex, markIndex);
            }
        },
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .slide-viewer-annotation-ui {
        min-height: 90px;
        max-height: 650px;
        width: 350px;
        margin-right: 10px;
        background-color: $nhsuk-white;
        border-radius: 5px;
        border: 1px solid $nhsuk-grey;
        flex-direction: column;
        align-items: center;
        font-size: 16px;
        padding-top: 5px;

        .annotation-ui-header {
            display: flex;
            flex: 1 0 auto;
            text-align: left;
            border-bottom: $nhsuk-grey-light 1px solid;
            padding: 10px 10px 10px 20px;
            width: 100%;
            min-height: 30px;
        }

        .annotation-content-and-footer {
            width: 100%;
            display: flex;
            flex: 1 0 auto;
            flex-direction: column;

            .annotation-content {
                max-height: 400px;
                padding: 10px;
                flex: 1 0 auto;

                .annotation-content-list-item {
                    margin-bottom: 8px;
                }
            }

            .annotation-ui-footer {
                flex: 1 0 auto;
                border-top: $nhsuk-grey-light 1px solid;
                padding: 10px;
                width: 100%;
                text-align: center;

                .annotation-ui-add-annotation-disabled {
                    background-color: $nhsuk-grey-light;
                    border-radius: 5px;
                    color: $nhsuk-white;
                    border-color: $nhsuk-grey-light;
                }
            }
        }
    }
</style>
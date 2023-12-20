<template>
    <div class="annotation-item">
        <div class="annotation-item-selected">
            <div v-if="currentlySelected"
                 class="annotation-item-pin-icon">
             <span class="fa-stack">
                  <span class="fas fa-map-marker fa-stack-2x"></span>
                  <strong class="fa-stack-1x annotation-item-pin-number">
                      {{ order + 1 }}    
                  </strong>         
            </span>
            </div>
            <div v-else
                 class="annotation-item-circle-icon">
                <div class="circle-icon">
                    <strong class="annotation-item-circle-number">
                        {{ order + 1 }}
                    </strong>
                </div>
            </div>
            <div v-if="feedbackVisible">
                <div v-if="answerStatus === AnswerTypeEnum.Best">
                    <span class="fa-stack">
                        <span class="fa-solid fa-circle-check green fa-stack-2x"></span>
                    </span>
                </div>
                <div v-else>
                    <span class="fa-stack">
                        <span class="fas fa-times-circle fa-stack-2x"></span>
                    </span>
                </div>
            </div>

            <div class="annotation-details">
                <div class="annotation-details-header">
                    <div class="annotation-details-header-name">
                        <strong @click="showDetails">{{ label }}</strong>
                    </div>
                    <div v-if="isEditable"
                         class="annotation-details-header-options">
                        <AnnotationItemOptions :first-in-list="order === 0"
                                               :last-in-list="order === lastIndexInList"
                                               :show-menu="optionOpenOrClose"
                                               @delete="this.delete"
                                               @edit="this.edit"
                                               @moveDown="this.moveAnnotationDown"
                                               @moveUp="this.moveAnnotationUp"
                                               @optionsClicked="this.optionsClicked"/>
                    </div>
                </div>
                <div>
                    <div class="annotation-detail-description">
                        {{ description }}
                    </div>
                    <div v-if="currentlySelected">
                        <div v-for="(mark, index) in this.annotationMarks">
                            <AnnotationItemMark :annotation-colour="annotation.colour"
                                                :annotation-mark="mark"
                                                :index="index"
                                                :is-editable="isEditable"
                                                :mark-is-selected="index === selectedMarkIndex"
                                                :mark-option-open="index === indexOfOpenMarkOptions"
                                                @deleteMark="deleteMark"
                                                @markOptionsClicked="setOpenMarkOptionIndex"
                                                @renameMark="closeMarkOptionMenu"
                                                @selectAMark="selectAMark"/>
                        </div>
                        <div v-if="currentlyEditing"
                             class="annotation-details-being-edited">
                            <div class="annotation-details-editing-statement">
                                <strong>Add a label and description</strong>
                            </div>
                        </div>
                        <div v-if="!placed"
                             ref="newestPin"
                             class="annotation-new">
                            <SlideViewerAddNewAnnotation class="annotation-new-controls"
                                                         @newAnnotationDiscard="$emit('newAnnotationDiscard')"
                                                         @newAnnotationDone="$emit('newAnnotationDone')"/>
                        </div>
                        <div v-if="currentlyViewingDetails"
                             class="annotation-marks">
                            <div v-if="isEditable"
                                 class="annotation-mark-control">
                                <div v-if="drawingModeEnabled"
                                     class="annotation-mark-draw">
                                    <AnnotationNewMark :is-drawing-mark="drawingModeEnabled"
                                                       :selected-colour="annotation.colour"
                                                       @colourSelected="setNewColour"
                                                       @stopDrawing="$emit('stopDrawing')"/>
                                </div>
                                <div v-else-if="pinBeingMoved"
                                     class="annotation-move-pin">
                                    <div>
                                        <strong>Place pin for location of the annotation</strong>
                                    </div>
                                    <div class="annotation-move-pin-button">
                                        <Button color="green"
                                                size="small" class="nhsuk-u-font-size-16"
                                                @click="$emit('pinDoneMoving')">
                                            Done
                                        </Button>
                                    </div>
                                </div>
                                <div v-else
                                     class="annotation-mark-control-commands">
                                    <div class="annotation-mark-control-draw-mark">
                                        <Button size="small" class="nhsuk-u-font-size-16"
                                                @click="$emit('startDrawing', order)">
                                            Start Drawing
                                        </Button>
                                    </div>
                                    <div class="annotation-mark-control-move-pin">
                                        <Button size="small" class="nhsuk-u-font-size-16"
                                                @click="this.movePin">
                                            Move Pin
                                        </Button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import LinkTextAndIcon from '../../globalcomponents/LinkTextAndIcon.vue';
    import SlideViewerAddNewAnnotation from '../../resource/blocks/SlideViewerAddNewAnnotation.vue';
    import Button from '../../globalcomponents/Button.vue';
    import AnnotationItemOptions from '../../resource/blocks/AnnotationItemOptions.vue';
    import AnnotationNewMark from "../../resource/blocks/AnnotationNewMark.vue";
    import { ImageAnnotationMarkModel } from "../../models/contribute-resource/blocks/annotations/imageAnnotationMarkModel";
    import AnnotationItemMark from "./AnnotationItemMark.vue";
    import { ImageAnnotation } from "../../models/contribute-resource/blocks/annotations/imageAnnotationModel";
    import { ImageAnnotationColourEnum } from "../../models/contribute-resource/blocks/annotations/imageAnnotationColourEnum";
    import { AnswerTypeEnum } from "../../models/contribute-resource/blocks/questions/answerTypeEnum";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";

    export default Vue.extend({
        components: {
            AnnotationItemMark,
            LinkTextAndIcon,
            SlideViewerAddNewAnnotation,
            Button,
            AnnotationItemOptions,
            AnnotationNewMark,
        },
        props: {
            label: String,
            order: Number,
            description: String,
            placed: Boolean,
            currentlyEditing: Boolean,
            currentlySelected: Boolean,
            currentlyViewingDetails: Boolean,
            pinBeingMoved: Boolean,
            lastIndexInList: Number,
            optionOpenOrClose: { type: Boolean, default: false },
            isEditable: Boolean,
            drawingModeEnabled: Boolean,
            annotationMarks: { type: Array } as PropOptions<ImageAnnotationMarkModel[]>,
            annotation: { type: Object } as PropOptions<ImageAnnotation>,
            selectedMarkIndex: Number,
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            feedbackVisible: { type: Boolean, default: false } as PropOptions<boolean>,
        },
        data() {
            return {
                indexOfOpenMarkOptions: -1,
                AnswerTypeEnum: AnswerTypeEnum,
                answerStatus: this.answers?.find(a => a.imageAnnotationOrder === this.order)?.status,
            };
        },
        mounted() {
            if (!this.placed) {
                let vue = this;

                Vue.nextTick(function () {
                    let element = vue.$refs["newestPin"] as Element;
                    if (element != null) {
                        // Scroll relevant item into view, then scroll the screen back to start
                        element.scrollIntoView({ block: 'nearest', inline: 'nearest' });
                        let startElement = document.getElementById("annotation-edit-start");
                        if (startElement != null) {
                            startElement.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'start' });
                        }
                    }
                });
            }
        },
        methods: {
            edit() {
                this.$emit('editAnnotationClicked', this.order);
                this.closeMarkOptionMenu();
            },
            delete() {
                this.$emit('deleteAnnotationClicked', this.order);
                this.closeMarkOptionMenu();
            },
            showDetails() {
                this.$emit('showDetails', this.order);
                this.closeMarkOptionMenu();
            },
            movePin() {
                this.$emit('movePin', this.order);
                this.closeMarkOptionMenu();
            },
            optionsClicked() {
                this.$emit('optionsClicked', this.order);
                this.closeMarkOptionMenu();
            },
            moveAnnotationUp() {
                this.$emit('moveAnnotationUp', this.order);
                this.closeMarkOptionMenu();
            },
            moveAnnotationDown() {
                this.$emit('moveAnnotationDown', this.order);
                this.closeMarkOptionMenu();
            },
            deleteMark(index: number) {
                this.annotation.deleteAnnotationMark(this.annotation.imageAnnotationMarks[index]);
                this.$emit('stopDrawing');
                this.$emit('annotationMarkDeleted');
                this.selectAMark(-1);
                this.closeMarkOptionMenu();
            },
            setOpenMarkOptionIndex(index: number) {
                if (this.indexOfOpenMarkOptions === index) {
                    this.closeMarkOptionMenu();
                } else {
                    this.selectAMark(index);
                    this.$emit('stopDrawing');
                    this.indexOfOpenMarkOptions = index;
                }
            },
            closeMarkOptionMenu() {
                this.indexOfOpenMarkOptions = -1;
            },
            setNewColour(colourIndex: number) {
                this.annotation.colour = colourIndex as ImageAnnotationColourEnum;
                this.$emit('annotationColourChanged');
            },
            selectAMark(markIndex: number) {
                this.$emit('selectAMark', this.order, markIndex);
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .green {
        color: $nhsuk-green;
    }

    .annotation-item {
        width: 100%;
        display: flex;
        min-height: 45px;

        .annotation-item-selected {
            display: flex;
        }

        .annotation-item-not-selected {
            display: flex;
        }

        .annotation-item-pin-icon {
            width: 50px;
            display: flex;
            justify-content: center;

            .fa-map-marker {
                color: $nhsuk-blue;
            }

            .annotation-item-pin-number {
                font-size: small;
                margin-top: -3px;
                color: $nhsuk-white;
            }
        }

        .annotation-item-circle-icon {
            width: 50px;
            display: flex;
            justify-content: center;

            .circle-icon {
                font-size: 3em;
                border-radius: 50%;
                border: 1px solid black;
                color: black;
                line-height: 2px;
                width: 30px;
                height: 30px;
                text-align: center;
                display: inline-block;


                .annotation-item-circle-number {
                    font-size: small;
                    color: $nhsuk-black;
                    margin-top: -10px;
                }
            }
        }

        .annotation-details {
            width: 260px;
            max-width: 100%;

            .annotation-details-header {
                display: flex;
                justify-content: space-between;

                .annotation-details-header-name {
                    white-space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    width: 185px;
                    flex: 4 0 auto;
                }

                .annotation-details-header-options {
                    flex: 2 0 auto;
                    text-align: end;
                    position: relative;
                }
            }

            .annotation-new {
                display: flex;

                .annotation-new-controls {
                    justify-content: center;
                }
            }

            .annotation-detail-description {
                margin-top: 5px;
                max-height: 48px;
                max-width: 100%;
                display: -webkit-box;
                -webkit-line-clamp: 2; //Show ... after the second line
                -webkit-box-orient: vertical;
                overflow: hidden;
                text-overflow: ellipsis;
                flex-basis: 100%;
                flex-shrink: 0;

                .annotation-detail-description-empty {
                    color: $nhsuk-grey-light
                }
            }

            .annotation-details-being-edited {
                .annotation-details-editing-statement {
                    padding: 10px 5px 5px 10px;
                    background-color: $nhsuk-white;
                    border: 3px solid $nhsuk-green;
                    text-align: center;
                }
            }

            .annotation-marks {
                width: 100%;

                .annotation-mark-control {

                    .annotation-mark-control-commands {
                        display: inline-flex;
                        width: 100%;
                        justify-content: space-around;
                    }

                    .annotation-move-pin {
                        height: auto;
                        width: 260px;
                        margin-right: 10px;
                        background-color: $nhsuk-white;
                        border-radius: 5px;
                        border: 2px solid $nhsuk-green;
                        flex-direction: column;
                        align-items: center;
                        font-size: 16px;
                        padding: 15px;

                        .annotation-move-pin-button {
                            margin-top: 10px;
                            text-align: center;
                        }
                    }
                }
            }
        }
    }
</style>
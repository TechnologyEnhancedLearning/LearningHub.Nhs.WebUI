<template>
    <div class="annotation-detail-edit">
        <div class="annotation-detail-edit-sidebar">
            <div class="annotation-item-pin-icon">
                <span class="fa-stack">
                    <span class="fas fa-map-marker fa-stack-2x"></span>
                    <strong class="fa-stack-1x annotation-item-pin-number">
                        {{ annotationToEdit.order + 1 }}
                    </strong>
                </span>
            </div>
        </div>
        <div class="annotation-detail-edit-content">
            <div class="annotation-edit-label">
                <div>
                    <strong>Label</strong>
                </div>
                <div>
                    <CharacterCount v-model="annotationToEdit.label"
                                    :characterLimit="60" />
                </div>
            </div>
            <div class="annotation-edit-description">
                <div>
                    <strong>Description</strong> (optional)
                </div>
                <div>
                    <CharacterCount v-model="annotationToEdit.description"
                                    placeholder="Add an additional description here"
                                    :rows="5"
                                    :characterLimit="400" />
                </div>
            </div>
            <div v-if="imageZone" class="pb-3">
                <div>
                    <strong>Is this answer option correct?</strong>
                </div>
                <div class="align-items-center">
                    <label class="my-0 pl-4 label-text">
                        <input class="radio-button"
                               type="radio"
                               :value="AnswerTypeEnum.Best"
                               v-model="answer.status" />
                        Yes
                    </label>
                    <label class="my-0 pl-4 label-text">
                        <input class="radio-button"
                               type="radio"
                               :value="AnswerTypeEnum.Incorrect"
                               v-model="answer.status" />
                        No
                    </label>
                </div>
            </div>
            <div class="w-100 d-inline-flex">
                <Button class="annotation-edit-button-done mr-2"
                        size="small"
                        color="green"
                        @click="done">
                    Done
                </Button>
                <Button class="annotation-edit-button-cancel"
                        size="small"
                        @click="cancel">
                    Cancel
                </Button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Button from '../../globalcomponents/Button.vue';
    import CharacterCount from '../../globalcomponents/CharacterCount.vue';
    import { ImageAnnotation } from "../..//models/contribute-resource/blocks/annotations/imageAnnotationModel";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import { AnswerTypeEnum } from "../../models/contribute-resource/blocks/questions/answerTypeEnum";


    export default Vue.extend({
        name: "AnnotationDetailEdit",
        components: {
            CharacterCount,
            Button
        },
        props: {
            annotationToEdit: { type: Object } as PropOptions<ImageAnnotation>,
            annotationIndex: Number,
            forceCancel: Boolean,
            answer: { type: Object } as PropOptions<AnswerModel>,
            imageZone: Boolean,
        },
        data() {
            return {
                initialAnnotation: Object.assign({}, this.annotationToEdit),
                AnswerTypeEnum: AnswerTypeEnum

            };
        },
        methods: {

            done() {
                this.$emit('editDone');
            },
            cancel() {
                this.$emit('editCancel', this.initialAnnotation);
            },
        },
        watch: {
            forceCancel: function (newVal, oldVal) {
                this.cancel();
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .annotation-detail-edit {
        width: 350px;
        margin-right: 10px;
        background-color: $nhsuk-white;
        border-radius: 5px;
        border: 1px solid $nhsuk-grey;
        display: flex;
        font-size: 16px;
        padding: 15px;

        .annotation-detail-edit-sidebar {
            flex: 1;

            .annotation-item-pin-icon {
                .fa-map-marker {
                    color: $nhsuk-blue;
                }

                .annotation-item-pin-number {
                    font-size: small;
                    margin-top: -3px;
                    color: $nhsuk-white;
                }
            }
        }

        .annotation-detail-edit-content {
            flex: 6;
            padding: 5px;

            .annotation-edit-buttons {
                display: flex;
                justify-content: flex-end;
            }
        }
    }
</style>
<template>
    <div class="annotation-new-mark">
        <div class="new-mark-header">
            <div>
                <strong>Select a shape to draw</strong>
            </div>
            <div>
                <LinkTextAndIcon @click="$emit('stopDrawing')"> Stop drawing</LinkTextAndIcon>
            </div>
        </div>
        <div class="new-mark-drawing-options">
            <div class="new-mark-drawing-shape">
                <span class="fa-stack">
                    <i class="fa-regular fa-circle fa-stack-2x"></i>
                    <i class="fa-solid fa-wave-square fa-stack-1x"></i>
                </span>
                <span class="fa-stack">
                    <i class="fa-regular fa-circle fa-stack-2x"></i>
                    <i class="fa-regular fa-square fa-stack-1x disabled"></i>
                </span>
                <span class="fa-stack">
                    <i class="fa-regular fa-circle fa-stack-2x"></i>
                    <i class="fa-regular fa-circle fa-stack-1x disabled"></i>
                </span>
            </div>
            <div class="new-mark-drawing-colour">
                <div v-for="(colourKey) in this.colourKeys">
                    <div class="colour-container"
                         :class="selectedColour === colourKey ? 'selected-colour-border' : ''">
                        <i :class="'fas fa-circle colour-select-' + getColourName(colourKey)"
                           tabindex="0"
                           @keyup.enter="selectColour(colourKey)"
                           @click="selectColour(colourKey)"></i>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import LinkTextAndIcon from "../../globalcomponents/LinkTextAndIcon.vue";
    import { ImageAnnotationColourEnum } from "../../models/contribute-resource/blocks/annotations/imageAnnotationColourEnum";

    export default Vue.extend({
        components: {
            LinkTextAndIcon,
        },
        props: {
            drawingModeEnabled: Boolean,
            selectedColour: Number,
        },
        data() {
            return {
                colourKeys: Object.values(ImageAnnotationColourEnum).filter(v => !isNaN(Number(v))),
            };
        },
        methods: {
            selectColour(colourIndex: number) {
                this.$emit("colourSelected", colourIndex);
            },
            getColourName(colourKey: number): string {
                return ImageAnnotationColourEnum[colourKey].toLowerCase();
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use "../../../../Styles/abstracts/all" as *;

    .annotation-new-mark {
        width: 260px;
        margin-right: 10px;
        background-color: $nhsuk-white;
        border: 3px solid $nhsuk-green;
        flex-direction: column;
        align-items: center;
        font-size: smaller;
        padding: 15px 5px 15px 5px;
        display: flex;
        justify-content: space-between;

        .new-mark-header {
            width: 100%;
            display: flex;
            flex-direction: row;
            justify-content: space-between;
        }

        .new-mark-drawing-options {
            width: 100%;
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            margin-top: 5px;

            .new-mark-drawing-shape {
                font-size: 16px;

                .fa-stack {
                    vertical-align: top;
                }

                .fal {
                    color: $nhsuk-grey-light;
                }

                .disabled {
                    color: $nhsuk-grey-light;
                }
            }

            .new-mark-drawing-colour {
                background-color: $nhsuk-grey-lighter;
                border: 2px solid $nhsuk-grey;
                display: flex;
                flex: 0 0 90px;
                align-items: center;
                justify-content: center;

                .colour-select-black {
                    color: $nhsuk-black;
                }

                .colour-select-white {
                    color: $nhsuk-white;
                }

                .colour-select-red {
                    color: $nhsuk-red;
                }

                .colour-select-green {
                    color: $nhsuk-green;
                }

                .colour-container {
                    display: inline-grid;
                    align-items: center;
                    padding: 1px;
                    margin: 2px;
                }

                .selected-colour-border {
                    border-radius: 50%;
                    background-color: $nhsuk-white;
                    border: 2px solid $nhsuk-blue;
                }
            }
        }
    }
</style>
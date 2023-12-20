<template>
    <div class="wsi-block"
         :class="{ 'wsi-block-fullscreen': isFullScreen }"
         ref="wholeSlideImageBlock">
        <div class="wsi-block-title">
            <h3 class="nhsuk-heading-m m-0">
                {{ imageZone ? block.wholeSlideImageBlock.wholeSlideImageBlockItems[0].wholeSlideImage.title : block.title }}</h3>
            <Button size="medium"
                    @click="toggleFullscreen"
                    class="wsi-block-full-screen-button"
                    color="blue">
                {{ isFullScreen ? 'Exit Full Screen' : 'Full Screen' }}
            </Button>
        </div>
        <div class="wsi-block-viewer-and-selector">
            <SlideViewerWithAnnotations :whole-slide-image="selectedSlide"
                                        :is-editable="false"
                                        :controls-visible="true"
                                        :is-full-screen="isFullScreen"
                                        :disabled="disabled"
                                        :answers="answers"
                                        :feedbackVisible="feedbackVisible"
                                        :imageZone="imageZone"
                                        :selectedAnswersProperty="selectedAnswersProperty"
                                        @updateSelectedAnswersProperty="updateSelectedAnswersProperty"
                                        v-if="selectedSlide.isReadyToPublish()"
                                        class="wsi-block-viewer"/>
            <WholeSlideImagePlaceholder v-else
                                        :placeholderText="getBlockItemBySlide(selectedSlide).placeholderText"
                                        :is-full-screen="isFullScreen"
                                        class="wsi-block-viewer"/>
            <SlideDrawer ref="slideDrawer"
                         v-if="!imageZone"
                         :whole-slide-images="wholeSlideImages"
                         :selected-slide="selectedSlide"
                         :is-full-screen="isFullScreen"
                         @chooseSlide="chooseSlide"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import SlideDrawer from './SlideDrawer.vue';
    import Button from '../../globalcomponents/Button.vue';
    import { BlockModel } from '../../models/contribute-resource/blocks/blockModel';
    import { WholeSlideImageModel } from '../../models/contribute-resource/blocks/wholeSlideImageModel';
    import {
        addListenerToDocument,
        requestFullscreen,
        exitFullscreen,
        isNotFullscreenMode
    } from '../../helpers/fullScreenListenerHelper';
    import SlideViewerWithAnnotations from "./SlideViewerWithAnnotations.vue";
    import WholeSlideImagePlaceholder from "./WholeSlideImagePlaceholder.vue";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";


    export default Vue.extend({
        components: {
            SlideViewerWithAnnotations,
            Button,
            SlideDrawer,
            WholeSlideImagePlaceholder,
        },
        props: {
            block: { type: Object } as PropOptions<BlockModel>,
            imageZone: Boolean,
            selectedAnswersProperty: { type: Array } as PropOptions<number[]>,
            disabled: Boolean,
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            feedbackVisible: { type: Boolean, default: false } as PropOptions<boolean>,
        },
        data() {
            const block = this.block as BlockModel;
            const firstBlock = (block.wholeSlideImageBlock.wholeSlideImageBlockItems
                    && block.wholeSlideImageBlock.wholeSlideImageBlockItems
                    && block.wholeSlideImageBlock.wholeSlideImageBlockItems[0].wholeSlideImage)
                || undefined;

            return {
                isFullScreen: false,
                selectedSlide: firstBlock,
            };
        },
        created() {
            this.block.wholeSlideImageBlock.wholeSlideImageBlockItems.forEach(blockItem => {
                if (blockItem.isPlaceholderWithoutTitle()) {
                    blockItem.wholeSlideImage.title = "This slide isn't ready yet";
                }
            });
        },
        computed: {
            wholeSlideImages(): WholeSlideImageModel[] {
                const block = this.block as BlockModel;
                return block.wholeSlideImageBlock.wholeSlideImageBlockItems.map(bi => bi.wholeSlideImage);
            }
        },
        mounted() {
            const fullscreenChangeEventListener = (element: string) => () => {
                this.isFullScreen = !!(document as any)[element];
                if (this.$refs.slideDrawer) {
                    window.setTimeout(() => {
                        (this.$refs.slideDrawer as any).toggledFullscreen();
                    }, 100);
                }
            };
            addListenerToDocument(fullscreenChangeEventListener);
        },
        methods: {
            async toggleFullscreen(): Promise<void> {
                if (isNotFullscreenMode()) {
                    const wholeSlideImageBlock = this.$refs.wholeSlideImageBlock as any;
                    await requestFullscreen(wholeSlideImageBlock);
                    this.isFullScreen = true;
                } else {
                    await exitFullscreen();
                    this.isFullScreen = false;
                }
            },

            chooseSlide(slide: WholeSlideImageModel): void {
                this.selectedSlide = slide;
            },

            getBlockItemBySlide(slide: WholeSlideImageModel) {
                const block = this.block as BlockModel;
                const index = this.wholeSlideImages.indexOf(slide);
                return block.wholeSlideImageBlock.wholeSlideImageBlockItems[index];
            },

            updateSelectedAnswersProperty(selectedAnswersProperty: number[]) {
                this.$emit("updateSelectedAnswersProperty", selectedAnswersProperty);
            }
        },
    });
</script>

<style lang="scss"
       scoped>
    @use "../../../../Styles/abstracts/all" as *;

    .wsi-block {
        border: solid 1px $nhsuk-grey-light;
        border-radius: 6px 6px 0 0;
        overflow: hidden; // prevents the title spilling out across the border-radius

        .wsi-block-title {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background-color: $nhsuk-grey-white;
            padding: 15px;

            @media screen and (min-width: 768px) and (min-height: 768px) {
                padding: 25px;
            }
        }

        .wsi-block-viewer-and-selector {
            position: relative;
        }

        .wsi-block-viewer {
            height: 756px;
        }

        &.wsi-block-fullscreen {
            height: 100vh;
            display: flex;
            flex-direction: column;
            border: none;
            border-radius: 0;

            .wsi-block-title {
                padding: 15px;
            }

            .wsi-block-viewer-and-selector {
                flex-grow: 1;
                flex-shrink: 1;
                display: flex;
                flex-direction: column;
                align-items: stretch;
            }

            .wsi-block-viewer {
                flex-grow: 1;
                flex-shrink: 1;
                height: auto;
                max-height: unset;
            }

            @media screen and (orientation: landscape) {
                .wsi-block-viewer-and-selector {
                    height: 100%;
                    flex-direction: row-reverse;
                }
            }

            @media screen and (orientation: portrait) {
                .wsi-block-viewer-and-selector {
                    flex-direction: column;
                    width: 100%;
                }
            }
        }
    }
</style>

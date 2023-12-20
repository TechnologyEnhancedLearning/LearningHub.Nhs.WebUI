<template>
    <div class="image-carousel-block"
         v-bind:class="{ 'image-carousel-block-fullscreen': isFullScreen }"
         ref="imageCarouselBlock">
        <div class="image-carousel-header" ref="imageCarouselTitle">
            <div>
                <h3 class="nhsuk-heading-m m-0">{{this.block.title}}</h3>
                <div>
                    {{this.imageCarouselDescription}}
                </div>
            </div>
            <div class="image-carousel-controls">
                <div class="displayed-slide">
                    Displaying {{displayedIndexNumber}} of {{imageCarouselImageBlocks.length}}
                </div>
                <div class="play-button-wrapper">
                    <button class="btn play-button play-button-shape" aria-label="toggle play" @click="togglePlay">
                        <i v-if="!isAutoPlay" class="fas fa-play fa-2x"></i>
                        <i v-else class="fa fa-pause fa-2x"></i>
                    </button>
                </div>
                <Button size="medium"
                        v-on:click="toggleFullscreen"
                        color="blue">
                    {{ isFullScreen ? 'Exit Full Screen' : 'Full Screen' }}
                </Button>
            </div>
        </div>
        <div class="image-carousel-viewer-and-selector" :class="this.isFullScreen ? 'image-carousel-viewer-and-selector-fullscreen' : ''">
            <ImageCarouselSlideSelector v-if="isFullScreen"
                                        ref="slideDrawer"
                                        class="slide-selector"
                                        :image-block-collection="imageCarouselImageBlockCollection"
                                        :selected-slide-index="activeSlideIndex"
                                        :is-full-screen="isFullScreen"
                                        v-on:slideChanged="setActiveSlide"/>
            <ImageCarouselViewer class="image-carousel-viewer" 
                                 v-bind:image-block-collection="imageCarouselImageBlockCollection" 
                                 v-bind:slide-height="slideHeight"
                                 v-bind:slide-width="slideWidth"
                                 v-bind:active-slide-index="activeSlideIndex"
                                 v-bind:is-auto-play="isAutoPlay"
                                 v-bind:is-fullscreen="isFullScreen"
                                 v-on:slideChanged="setActiveSlide"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    import { ImageCarouselBlockModel } from "../../models/contribute-resource/blocks/imageCarouselBlockModel";
    import ImagePublishedView from "../../contribute-resource/components/published-view/ImagePublishedView.vue";
    import Vuetify from "vuetify";
    import ImageCarouselViewer from "./ImageCarouselViewer.vue";
    import { BlockCollectionModel } from "../../models/contribute-resource/blocks/blockCollectionModel";
    import Button from "../../globalcomponents/Button.vue";
    import {
        addListenerToDocument,
        requestFullscreen,
        exitFullscreen,
        isNotFullscreenMode
    } from '../../helpers/fullScreenListenerHelper';
    import SlideDrawer from "./SlideDrawer.vue";
    import ImageCarouselSlideSelector from "./ImageCarouselSlideSelector.vue";
    import IconButton from "../../globalcomponents/IconButton.vue";
    Vue.use(Vuetify);
    
    export default Vue.extend({
        components: {
            IconButton,
            ImageCarouselSlideSelector, SlideDrawer, Button, ImageCarouselViewer, ImagePublishedView },
        props: {
            block: { type: Object } as PropOptions<BlockModel>,
        },
        data() {
            return {
                isFullScreen: false,
                isLandscape: true,
                activeSlideIndex: 0,
                isAutoPlay: false,
                displayedIndexNumber: 1,
            }
        },
        mounted() {
            const fullscreenChangeEventListener = (element: string) => () => {
                this.isFullScreen = !!(document as any)[element];
                window.setTimeout(() => {}, 100);
            }
            addListenerToDocument(fullscreenChangeEventListener);
            
            window.onresize = () => {
                this.setIsLandscape();
            }

            this.setIsLandscape();
        },
        computed: {
            imageCarouselBlock(): ImageCarouselBlockModel {
                return this.block.imageCarouselBlock;
            },
            imageCarouselDescription(): string {
                return this.imageCarouselBlock?.description;
            },
            imageCarouselImageBlocks(): BlockModel[] {
                return this.imageCarouselBlock?.imageBlockCollection.blocks;
            },
            imageCarouselImageBlockCollection(): BlockCollectionModel {
                return this.imageCarouselBlock?.imageBlockCollection;
            },
            slideHeight(): number {
                if (this.isFullScreen) {
                    if (this.isLandscape) {
                        return window.innerHeight;
                    } else {
                        // Slide selector has fixed height of 250px, so remove this in landscape mode
                        return window.innerHeight - 250; 
                    }
                } else {
                    return 600;
                }
            },
            slideWidth(): number {
                if (this.isFullScreen) {
                    return window.innerWidth * 0.5; // Max slide width in fullscreen mode is half the screen size.
                } else {
                    // Set max width of slide in non-fullscreen mode to 500px
                    return 500;
                }
            },
        },
        methods: {
            async toggleFullscreen(): Promise<void> {
                if (isNotFullscreenMode()) {
                    const imageCarouselBlock = this.$refs.imageCarouselBlock as any;
                    await requestFullscreen(imageCarouselBlock);
                    this.isFullScreen = true;
                } else {
                    await exitFullscreen();
                    this.isFullScreen = false;
                }
            },
            setIsLandscape() {
                this.isLandscape = (document.documentElement.clientWidth > document.documentElement.clientHeight);
            },
            setActiveSlide(index: number) {
                if (index != -1) {
                    this.displayedIndexNumber = index + 1;
                }
                
                this.activeSlideIndex = index;
            },
            togglePlay() {
                this.isAutoPlay = !this.isAutoPlay;
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .image-carousel-block {
        border: solid 1px $nhsuk-grey-light;
        border-radius: 6px 6px 0 0;
        overflow: hidden;

        &.image-carousel-block-fullscreen {
            display: flexbox;
            flex-direction: column;
            margin: 0;
        }

        .image-carousel-viewer-and-selector {
            display: flex;

            &.image-carousel-viewer-and-selector-fullscreen {
                height: 100%;
                min-height: 50vh;

                @media screen and (orientation: portrait) {
                    flex-direction: column-reverse;
                }

                @media screen and (orientation: landscape) {
                    flex-direction: row;
                }
            }

            .image-carousel-viewer {
                flex: 1 1 500px;
            }
        }

        .image-carousel-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background-color: $nhsuk-grey-white;
            padding: 15px;

            .image-carousel-controls {
                display: flex;
                flex-direction: row;

                .displayed-slide {
                    align-self: center;
                    margin: 10px;
                }

                .play-button-wrapper {
                    margin-right: 10px;
                    align-self: center;

                    .play-button {
                        width: 50px;
                        height: 50px;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        background-color: $nhsuk-grey-lighter;

                        &:focus {
                            border-color: $govuk-focus-highlight-yellow;
                            color: $nhsuk-black;
                            background-color: $govuk-focus-highlight-yellow;
                            box-shadow: 0 2px 0 $nhsuk-black;
                            border-bottom: 1px solid $nhsuk-black;
                        }

                        &.play-button-shape {
                            border-radius: 50%;

                            &:focus {
                                box-shadow: 0 0 0 3px #212b32 inset;
                                border: none;
                            }
                        }
                    }
                }
            }

            @media screen and (min-width: 768px) and (min-height: 768px) {
                padding: 25px;
            }
        }
    }
</style>
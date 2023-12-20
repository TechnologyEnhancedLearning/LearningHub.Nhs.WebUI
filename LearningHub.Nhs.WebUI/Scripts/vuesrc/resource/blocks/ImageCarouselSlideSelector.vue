<template>
    <div class="carousel-slide-selector"
         v-bind:class="{
            'carousel-slide-selector-fullscreen': isFullScreen,
         }">
        <ul class="carousel-slide-selector-items"
            ref="itemsScrollPane">
            <li v-for="(imageBlock, index) in this.imageBlocks"
                :key="index"
                v-bind:class="{ 'carousel-slide-selector-item-selected': (index === selectedSlideIndex) }">
                <ImageCarouselSlideSelectorIcon v-bind:image-block="imageBlock"
                                                v-on:slideChanged="$emit('slideChanged', index);" />
            </li>
        </ul>
    </div>
</template>

<script lang="ts">
    import Vue, {PropOptions} from "vue";
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    import {BlockCollectionModel} from "../../models/contribute-resource/blocks/blockCollectionModel";
    import ImagePublishedView from "../../contribute-resource/components/published-view/ImagePublishedView.vue";
    import ImageCarouselSlideSelectorIcon from "./ImageCarouselSlideSelectorIcon.vue";
    
    export default Vue.extend({
        components: {ImageCarouselSlideSelectorIcon, ImagePublishedView},
        props: {
            imageBlockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            selectedSlideIndex: Number,
            isFullScreen: Boolean,
        },
        computed: {
            imageBlocks(): BlockModel[] {
                return this.imageBlockCollection.blocks;
            },
        }
    });
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;
    
    .carousel-slide-selector {
        height: 340px;
        background-color: white;
        position: relative;
    
        .carousel-slide-selector-items {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            margin: 0;
            padding: 10px;
            overflow-x: scroll;
            display: flex;
            align-items: flex-start;
            list-style-type: none;
        }
    
        .carousel-slide-selector-opener {
            display: none;
        }
    
        .carousel-slide-selector-controls {
            position: absolute;
            top: 240px;
            left: 0;
            right: 0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 20px;
            pointer-events: none;
        }
    
        .carousel-slide-selector-button {
            pointer-events: all;
        }
    
        &.carousel-slide-selector-fullscreen {
            height: 100%;
            display: flex;
            flex-direction: column;
            border: none;
            border-radius: 0;
    
            .carousel-slide-selector-opener {
                position: absolute;
                display: block;
            }
    
            .carousel-slide-selector-controls {
                display: none;
            }
    
            @media screen and (orientation: landscape) {
                height: auto;
                width: 250px;
                transition: width 1s;
    
                .carousel-slide-selector-items {
                    flex-direction: column;
                    overflow-x: inherit;
                    overflow-y: scroll;
                    left: inherit;
                    width: 250px;
                    height: inherit;
    
                    button {
                        min-height: inherit;
                    }
                }
    
                .carousel-slide-selector-opener {
                    top: 20px;
                    right: -195px;
                    width: max-content;
                }
    
                &.carousel-slide-selector-closed {
                    width: 0;
                }
            }
    
            @media screen and (orientation: portrait) {
                height: 250px;
                width: auto;
                transition: height 1s;
    
                .carousel-slide-selector-opener {
                    top: -60px;
                    left: 20px;
                }
    
                &.carousel-slide-selector-closed {
                    height: 0;
                }
            }
        }
    
        .disabled-icon {
            align-content: center;
            font-size: 60px;
            color: $nhsuk-white;
            background-color: $nhsuk-blue;
            border-radius: 30%;
            padding: 19px;
            height: 98px;
            width: 98px;
        }
    }
</style>
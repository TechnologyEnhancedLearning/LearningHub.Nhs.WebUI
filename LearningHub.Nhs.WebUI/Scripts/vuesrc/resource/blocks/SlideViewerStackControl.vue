<template>
    <div class="slide-viewer-control">
        <div class="slide-viewer-control-title">Z Stack</div>
        <div class="slide-viewer-container">
            <div class="slide-viewer-control-slider slide-viewer-control-slider-progress"
                 :style="{ 'height': `${ getCurrentLayerHeight() }px`, 'transform': `translateY(${ getSliderPosition() }px)`}"></div>
            <div class="slide-viewer-control-slider"
                 v-on:mousedown="zstackSliderMouseDown">
                <div v-for="(index) in this.layers" class="slide-viewer-control-slider-position layer"
                     :class="{'slide-viewer-control-current-layer': (index - 1) === layer}"
                     @click="layer = index - 1"
                     :style="{ transform: `translateY(-${ getLayerHeight(index)}px)` }"
                     :key="index"/>
                <div class="slide-viewer-control-slider-position slide-viewer-control-current-slider-position"
                     v-bind:style="{ transform: `translateY(-${ getCurrentSliderHeight() }px)` }">
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Button from '../../globalcomponents/Button.vue';
    import IconButton from '../../globalcomponents/IconButton.vue';
    import SlideViewerRotateControl from './SlideViewerRotateControl.vue';
    
    export default Vue.extend({
        components: {
            Button,
            IconButton,
            SlideViewerRotateControl,
        },
        props: {
            layers: Number
        },
        data() {
            return {
                layer: 0,
                slideViewerContainerHeight: 121,
                layerMarkerHeight: 8
            }
        },
        watch: {
            layer() {
                this.$emit('setSlideImage', this.layer);
            }  
        },
        methods: {
            getLayerHeight(index: number) {
                return (this.slideViewerContainerHeight - this.layerMarkerHeight) * (this.layers - index) / (this.layers - 1);
            },
            getSliderPosition() {
                return this.slideViewerContainerHeight - this.getCurrentLayerHeight();
            },
            getCurrentLayerHeight() {
                return (this.layers - this.layer - 1) / (this.layers - 1) * this.slideViewerContainerHeight;
            },
            getCurrentSliderHeight() {
                return (this.layers - this.layer - 1) / (this.layers - 1) * (this.slideViewerContainerHeight - 5);
            },
            zstackSliderMouseDown(event: any): void {
                if (event.buttons === 1) {
                    const rect = event.target.getBoundingClientRect();
                    const y = rect.top - event.clientY + (ZOOM_SLIDER_SIZE - 2);
                    this.updateZstackBasedOnMouseCoordinates(y);
                }
            },
            updateZstackBasedOnMouseCoordinates(y: number) {
                // Algorithm to convert y-axis co-ordinate (0 at bottom) into layer number (0 at top).
                var newLayer = (this.layers - 1) - Math.round(y / ((ZOOM_SLIDER_SIZE - 2) / (this.layers - 1)));
                this.layer = newLayer;
            },
        }
    });

    const ZOOM_SLIDER_SIZE = 121; // Note: if we change this, also change the variable in CSS.
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;

    .slide-viewer-control {
        .layer {
            background-color: $nhsuk-grey-light;
            width: 60px;
            border-radius: 6px;
            height: 8px;
            z-index: 2;
        }

        .slide-viewer-control-slider,
        .slide-viewer-container {
            height: 121px;
            width: 88px;
        }

        .slide-viewer-control-slider {

            .slide-viewer-control-current-layer {
                background-color: $nhsuk-blue;
            }

            .slide-viewer-control-current-slider-position {
                left: 2px;
            }

            &:after {
                left: 10px;
                right: 70px;
            }
        }
    }
</style>

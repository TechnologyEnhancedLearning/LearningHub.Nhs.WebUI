<template>
    <div class="slide-viewer-control">
        <div class="slide-viewer-control-title">Zoom</div>

        <div class="slide-viewer-container">
            <div class="slide-viewer-control-slider slide-viewer-control-slider-progress"
                 :style="{ 'height': `${getZoomSliderHeight()}px`, 'transform': `translateY(${getZoomSliderProgress()}px)`}"></div>
            <div ref="zoomSlider"
                 v-on:mousedown="zoomSliderMouseDown"
                 class="slide-viewer-control-slider">
                <div ref="zoomSliderInner"
                     v-on:mousemove="zoomSliderMouseMove"
                     class="slide-viewer-control-slider-inner">
                </div>
                <div class="slide-viewer-control-slider-position"
                     v-bind:style="{ transform: `translateY(-${getZoomSliderPosition()}px)` }">
                </div>
            </div>
        </div>
        <div class="slide-viewer-control-value-and-buttons">
            <IconButton size="small"
                        shape="circle"
                        color="blue"
                        aria-label="Zoom out"
                        v-on:click="zoomOut"
                        icon-classes="fa-solid fa-minus"/>
            <span class="slide-viewer-control-value">
                {{ (zoomValue).toFixed(1) }}x
            </span>
            <IconButton size="small"
                        shape="circle"
                        color="blue"
                        aria-label="Zoom in"
                        v-on:click="zoomIn"
                        icon-classes="fa-solid fa-plus"/>
        </div>
        
        <a href="#"
           v-on:click.prevent="resetZoom"
           class="slide-viewer-control-reset accessible-link"
           aria-label="Reset zoom">
            Reset
        </a>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { Viewer } from 'openseadragon';
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
            viewer: {type: Object} as PropOptions<Viewer>,
        },
        data() {
            return {
                zoomValue: 1,
                zoomSliderValue: 0,
                isZoomSliderActive: false,
            }
        },
        watch: {
            viewer: function(newVal, oldVal) {
                this.viewer.addHandler('zoom', () => {
                    this.setZoomSliderLevel();
                });
            },
        },
        methods: {
            resetZoom(): void {
                this.zoom(this.viewer.viewport.getMinZoom());
            },
            zoomOut(): void {
                const minZoom = this.viewer.viewport.getMinZoom();
                const currentZoom = this.viewer.viewport.getZoom();
                let newZoom = currentZoom / 2;
                if (newZoom < minZoom) {
                    newZoom = minZoom;
                }
                this.zoom(newZoom);
            },
            zoomIn(): void {
                const maxZoom = this.viewer.viewport.getMaxZoom();
                const currentZoom = this.viewer.viewport.getZoom();
                let newZoom = currentZoom * 2;
                if (newZoom > maxZoom) {
                    newZoom = maxZoom;
                }
                this.zoom(newZoom);
            },
            zoomSliderMouseDown(event: any): void {
                if (event.buttons === 1) {
                    this.isZoomSliderActive = true;
                    const zoomSliderInnerElement = this.$refs.zoomSliderInner as HTMLElement;
                    zoomSliderInnerElement.style.pointerEvents = 'auto';

                    const rect = event.target.getBoundingClientRect();
                    const y = rect.top - event.clientY + (ZOOM_SLIDER_SIZE - 2);
                    this.zoomBasedOnMouseCoordinates(y);
                }
            },
            zoomSliderMouseMove(event: any): void {
                if (event.buttons !== 1) {
                    this.isZoomSliderActive = false;
                    const zoomSliderInnerElement = this.$refs.zoomSliderInner as HTMLElement;
                    zoomSliderInnerElement.style.pointerEvents = 'none';
                }

                if (this.isZoomSliderActive) {
                    const rect = event.target.getBoundingClientRect();
                    const y = rect.top - event.clientY + ZOOM_SLIDER_INNER_SIZE;
                    this.zoomBasedOnMouseCoordinates(y);
                }
            },
            zoomBasedOnMouseCoordinates(y: number) {
                let boundedY = y;
                if (boundedY < 0) { boundedY = 0; }
                if (boundedY > ZOOM_SLIDER_SIZE) { boundedY = ZOOM_SLIDER_SIZE; }

                const scaledY = boundedY / ZOOM_SLIDER_SIZE;

                const logMax = Math.log(this.viewer.viewport.getMaxZoom());
                const logZoomLevel = scaledY * logMax;
                const absoluteZoomLevel = Math.exp(logZoomLevel);

                if (this.viewer.viewport.getZoom() !== absoluteZoomLevel) {
                    this.zoom(absoluteZoomLevel);
                }
            },
            zoom(value: number): void {
                this.viewer.viewport.zoomTo(value);
                this.zoomValue = value;
            },
            setZoomSliderLevel(): void {
                const logMin = Math.log(this.viewer.viewport.getMinZoom());
                const logMax = Math.log(this.viewer.viewport.getMaxZoom());
                const logRange = logMax / logMin;
                const logZoomLevel = Math.log(this.viewer.viewport.getZoom());
                let sliderPosition = logZoomLevel / logRange / logMin;
                if (sliderPosition < 0) { sliderPosition = 0; }
                if (sliderPosition > 1) { sliderPosition = 1; }

                this.zoomSliderValue = sliderPosition;
                this.zoomValue = this.viewer.viewport.getZoom();
            },
            getZoomSliderHeight() {
                return this.zoomSliderValue * ZOOM_SLIDER_SIZE;
            },
            getZoomSliderProgress() {
                return ZOOM_SLIDER_SIZE - this.getZoomSliderHeight();
            },
            getZoomSliderPosition() {
                return (ZOOM_SLIDER_SIZE - 5) * this.zoomSliderValue;
            }
        },
    });
    
    const ZOOM_SLIDER_SIZE = 88; // Note: if we change this, also change the variable in CSS (in site.scss)
    const ZOOM_SLIDER_INNER_SIZE = 1000; // Note: if we change this, also change the variable in CSS (a few lines below this)
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;

    $ZOOM_SLIDER_INNER_SIZE: 1000px; // Note: if we change this, also change the variable in JS (a few lines above this)

    .slide-viewer-control {

        .slide-viewer-container {
            height: 88px;
            width: 88px;
            margin: 11px 0;
        }
        
        .slide-viewer-control-slider {

            .slide-viewer-control-slider-inner {
                position: absolute;
                bottom: 0;
                left: 50%;
                height: ($ZOOM_SLIDER_INNER_SIZE * 2);
                width: ($ZOOM_SLIDER_INNER_SIZE * 2);
                transform: translate(-$ZOOM_SLIDER_INNER_SIZE, $ZOOM_SLIDER_INNER_SIZE);
                pointer-events: none;
                z-index: 100;
                user-select: none;
            }

            .slide-viewer-control-slider-position {
                background-color: $nhsuk-blue;
                pointer-events: none;
            }
        }
    }
</style>

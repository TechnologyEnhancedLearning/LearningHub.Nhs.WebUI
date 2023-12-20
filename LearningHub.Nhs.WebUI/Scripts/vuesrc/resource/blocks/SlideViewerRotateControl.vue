<template>
    <div class="slide-viewer-control">
        <div class="slide-viewer-control-title">Rotate</div>
        <div class="slide-viewer-container">
            <div class="slide-viewer-rotate-control-dial slide-viewer-rotate-control-dial-progress"
                 :style="{ '--value': `${getAngleOfRotation()}%` }"/>
            <div ref="rotateDial"
                 v-on:mousedown="rotateDialMouseDown"
                 class="slide-viewer-rotate-control-dial">
                <div ref="rotateDialInner"
                     v-on:mousemove="rotateDialMouseMove"
                     class="slide-viewer-rotate-control-dial-inner">
                </div>
                <div class="slide-viewer-rotate-control-dial-position"
                     v-bind:style="{ transform: `rotate(${rotation}deg)` }">
                </div>
            </div>
        </div>

        <div class="slide-viewer-control-value-and-buttons">
            <IconButton size="small"
                        shape="circle"
                        color="blue"
                        aria-label="Rotate left"
                        v-on:click="rotateLeft"
                        icon-classes="fa-solid fa-rotate-left"/>
            <span class="slide-viewer-control-value">
                {{ rotation.toFixed(0) }}°
            </span>
            <IconButton size="small"
                        shape="circle"
                        color="blue"
                        aria-label="Rotate right"
                        v-on:click="rotateRight"
                        icon-classes="fa-solid fa-rotate-right"
                        class="slide-viewer-control-button-flip"/>
        </div>
        
        <a href="#"
           v-on:click.prevent="resetRotation"
           class="slide-viewer-control-reset accessible-link"
           aria-label="Reset rotation">
            Reset
        </a>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { Viewer } from 'openseadragon';
    import Button from "../../globalcomponents/Button.vue";
    import IconButton from "../../globalcomponents/IconButton.vue";

    export default Vue.extend({
        components: {
            Button,
            IconButton,
        },
        props: {
            viewer: {type: Object} as PropOptions<Viewer>,
        },
        data() {
            return {
                rotation: 0,
                isRotateDialActive: false,
            }
        },
        watch: {
            viewer(newVal, oldVal) {
                this.viewer.addHandler('rotate', (event) => {
                    const degreesNumber = event.degrees * 1;
                    const boundedDegrees = ((degreesNumber + 180) % 360) - 180;
                    this.rotation = boundedDegrees;
                });
            }
        },
        methods: {
            rotateLeft(): void {
                const rotationIncrement = 15;
                const currentRotation = this.viewer.viewport.getRotation();
                const snappedCurrentRotation = Math.floor(currentRotation / rotationIncrement) * rotationIncrement;
                const newRotation = snappedCurrentRotation - rotationIncrement;
                this.rotate(newRotation);
            },
            rotateRight(): void {
                const rotationIncrement = 15;
                const currentRotation = this.viewer.viewport.getRotation();
                const snappedCurrentRotation = Math.ceil(currentRotation / rotationIncrement) * rotationIncrement;
                const newRotation = snappedCurrentRotation + rotationIncrement;
                this.rotate(newRotation);
            },
            resetRotation(): void {
                this.rotate(0);
            },
            rotateDialMouseDown(event: any): void {
                if (event.buttons === 1) {
                    this.isRotateDialActive = true;
                    const rotateDialInnerElement = this.$refs.rotateDialInner as HTMLElement;
                    rotateDialInnerElement.style.pointerEvents = 'auto';

                    const rect = event.target.getBoundingClientRect();
                    const x = event.clientX - rect.left - 44;
                    const y = event.clientY - rect.top - 44;
                    this.rotateBasedOnMouseCoordinates(x, y);
                }
            },
            rotateDialMouseMove(event: any): void {
                if (event.buttons !== 1) {
                    this.isRotateDialActive = false;
                    const rotateDialInnerElement = this.$refs.rotateDialInner as HTMLElement;
                    rotateDialInnerElement.style.pointerEvents = 'none';
                }

                if (this.isRotateDialActive) {
                    const rect = event.target.getBoundingClientRect();
                    const x = event.clientX - rect.left - 1000;
                    const y = event.clientY - rect.top - 1000;
                    this.rotateBasedOnMouseCoordinates(x, y);
                }
            },
            rotateBasedOnMouseCoordinates(x: number, y: number) {
                const rad = Math.atan2(x, -y);
                const originalDeg = rad * (180 / Math.PI);
                let deg = originalDeg;

                if (-5 < deg && deg < 5) { deg = 0; }
                if (85 < deg && deg < 95) { deg = 90; }
                if (-95 < deg && deg < -85) { deg = -90; }
                if (175 < deg || deg < -175) { deg = 180; }

                this.rotate(deg);
            },
            rotate(deg: number): void {
                const boundedDegrees = ((deg + 180) % 360) - 180;
                this.viewer.viewport.setRotation(boundedDegrees);
                this.rotation = boundedDegrees;
            },
            getAngleOfRotation() {
                return this.rotation < 0 ? ((360 + this.rotation) / 360 * 100) : (this.rotation / 360 * 100);
            }
        },
    });
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;
    .slide-viewer-control {
        .slide-viewer-container,
        .slide-viewer-rotate-control-dial {
            width: 88px;
            height: 88px;
            margin: 11px 0;
        }

        .slide-viewer-rotate-control-dial-progress {
            background-image: conic-gradient($nhsuk-blue var(--value), $nhsuk-grey-light var(--value));
            z-index: 1;
            pointer-events: none;
        }

        .slide-viewer-rotate-control-dial {
            position: absolute;
            background-color: $nhsuk-grey-light;
            user-select: none;
            border-radius: 50%;

            &:after {
                content: '';
                position: absolute;
                top: 8px;
                left: 8px;
                bottom: 8px;
                right: 8px;
                background-color: $nhsuk-white;
                border-radius: 50%;
                pointer-events: none;
            }

            .slide-viewer-rotate-control-dial-inner {
                position: absolute;
                top: 50%;
                left: 50%;
                height: 2000px;
                width: 2000px;
                transform: translate(-1000px, -1000px);
                pointer-events: none;
                z-index: 100;
                user-select: none;
            }

            .slide-viewer-rotate-control-dial-position {
                position: absolute;
                top: -6px;
                left: 42px;
                height: 25px;
                width: 5px;
                background-color: $nhsuk-blue;
                transform-origin: 2px 51px;
                z-index: 1;
                pointer-events: none;
            }
        }
    }
</style>

<template>
    <div class="slide-drawer"
         v-bind:class="{
            'slide-drawer-closed': !isSlideDrawerOpen,
            'slide-drawer-fullscreen': isFullScreen,
         }">
        <div class="m-3">
            <Button size="medium"
                    v-on:click="isSlideDrawerOpen = !isSlideDrawerOpen"
                    class="slide-drawer-opener nhsuk-u-font-size-16">
                {{ isSlideDrawerOpen ? 'Hide Slide Drawer' : 'Show Slide Drawer' }}
            </Button>
        </div>
        <ul class="slide-drawer-items"
            ref="itemsScrollPane"
            v-on:scroll="scrolled">
            <li v-for="wholeSlideImage in wholeSlideImages"
                :key="wholeSlideImage.wholeSlideImageRef"
                class="slide-drawer-item"
                v-bind:class="{ 'slide-drawer-item-selected': (wholeSlideImage === selectedSlide) }">
                <button v-on:click="$emit('chooseSlide', wholeSlideImage); justSelected = true"
                        v-on:blur="justSelected = false"
                        v-bind:class="{ 'slide-drawer-item-just-selected': justSelected }">
                    <picture v-if="wholeSlideImage.getFileModel() && wholeSlideImage.getFileModel().wholeSlideImageFile">
                        <img v-bind:src="`/api/Resource/SlideImageTile/${wholeSlideImage.getFileModel().filePath}/0/8/0_0.jpg`" alt="" />
                    </picture>
                    <i class="fas fa-ban disabled-icon" v-else />
                    <span>{{ wholeSlideImage.title }}</span>
                </button>
            </li>
        </ul>
        <div class="slide-drawer-controls">
            <IconButton iconClasses="fa-solid fa-chevron-left"
                        ariaLabel="Scroll back"
                        shape="circle"
                        color="blue"
                        v-bind:disabled="showingFrom === 0"
                        v-on:click="scrollBack"
                        class="slide-drawer-button" />
            <div v-if="showingFrom !== undefined">
                Showing {{ (showingFrom + 1) }} to {{ (showingTo + 1) }} of {{ showingOf }}
            </div>
            <IconButton iconClasses="fa-solid fa-chevron-right"
                        ariaLabel="Scroll forward"
                        shape="circle"
                        color="blue"
                        v-bind:disabled="(showingTo + 1) === showingOf"
                        v-on:click="scrollForward"
                        class="slide-drawer-button" />
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Button from '../../globalcomponents/Button.vue';
    import IconButton from '../../globalcomponents/IconButton.vue';
    import { WholeSlideImageModel } from '../../models/contribute-resource/blocks/wholeSlideImageModel';

    export default Vue.extend({
        components: {
            Button,
            IconButton,
        },
        props: {
            wholeSlideImages: { type: Array } as PropOptions<WholeSlideImageModel[]>,
            selectedSlide: { type: Object } as PropOptions<WholeSlideImageModel>,
            isFullScreen: Boolean,
        },
        data() {
            return {
                isSlideDrawerOpen: true,
                scrollPosition: 0,
                showingFrom: undefined,
                showingTo: undefined,
                showingOf: undefined,
                justSelected: false,
            }
        },
        mounted() {
            window.addEventListener('resize', ev => {
                this.updateScrollPosition();
                this.updateShowing();
            });
            this.updateShowing();
        },
        methods: {
            scrolled(): void {
                const screenStats = this.calculateScreenStats();
                this.scrollPosition = screenStats.scrollPaneMiddle;
                this.updateShowing();
            },
            toggledFullscreen(): void {
                this.updateScrollPosition();
                this.updateShowing();
            },
            updateScrollPosition: function (): void {
                const screenStats = this.calculateScreenStats();
                const scrollAmountNeeded = this.scrollPosition - screenStats.scrollPaneHalfSize;
                this.scrollTo(scrollAmountNeeded);
            },
            updateShowing(): void {
                const screenStats = this.calculateScreenStats();
                const children = screenStats.scrollPane.querySelectorAll('.slide-drawer-item');

                let from = -1;
                let to = -1;

                for (let i = 0; i < children.length; i++) {
                    const child = children[i] as HTMLElement;
                    const childStats = this.calculateChildStats(child);

                    if (from === -1 && childStats.childLateHalfWayPoint >= screenStats.scrollPaneStart) {
                        from = i;
                    }
                    if (childStats.childEarlyHalfWayPoint <= screenStats.scrollPaneEnd) {
                        to = i;
                    }
                }

                if (from !== -1 && to !== -1) {
                    this.showingFrom = from;
                    this.showingTo = to;
                    this.showingOf = children.length;
                }
                else {
                    this.showingFrom = undefined;
                    this.showingTo = undefined;
                }
            },
            scrollBack(): void {
                const screenStats = this.calculateScreenStats();
                const children = screenStats.scrollPane.querySelectorAll('.slide-drawer-item');

                const child = children[this.showingFrom - 1] as HTMLElement;
                const childStats = this.calculateChildStats(child);

                const scrollAmountNeeded = childStats.childStart;
                this.scrollTo(scrollAmountNeeded);
            },
            scrollForward(): void {
                const screenStats = this.calculateScreenStats();
                const children = screenStats.scrollPane.querySelectorAll('.slide-drawer-item');

                const child = children[this.showingTo + 1] as HTMLElement;
                const childStats = this.calculateChildStats(child);

                const scrollAmountNeeded = childStats.childEnd - screenStats.scrollPaneSize;
                this.scrollTo(scrollAmountNeeded);
            },
            scrollTo(scrollAmountNeeded: number) {
                const screenStats = this.calculateScreenStats();
                if (screenStats.isLandscape) {
                    screenStats.scrollPane.scrollLeft = scrollAmountNeeded;
                } else {
                    screenStats.scrollPane.scrollTop = scrollAmountNeeded;
                }
            },
            calculateScreenStats() {
                const isLandscape = !this.isFullScreen || (document.documentElement.clientWidth < document.documentElement.clientHeight);
                const scrollPane = this.$refs.itemsScrollPane as HTMLElement;
                const scrollPaneStart = isLandscape ? scrollPane.scrollLeft : scrollPane.scrollTop;
                const scrollPaneSize = isLandscape ? scrollPane.offsetWidth : scrollPane.offsetHeight;
                const scrollPaneEnd = isLandscape ? (scrollPane.scrollLeft + scrollPaneSize) : (scrollPane.scrollTop + scrollPaneSize);
                const scrollPaneMiddle = scrollPaneStart + (scrollPaneSize / 2);
                const scrollPaneHalfSize = scrollPaneSize / 2;

                return {
                    isLandscape,
                    scrollPane,
                    scrollPaneStart,
                    scrollPaneSize,
                    scrollPaneEnd,
                    scrollPaneMiddle,
                    scrollPaneHalfSize,
                }
            },
            calculateChildStats(child: HTMLElement) {
                const screenStats = this.calculateScreenStats();
                const childStart = screenStats.isLandscape ? child.offsetLeft : child.offsetTop;
                const childSize = screenStats.isLandscape ? child.offsetWidth : child.offsetHeight;
                const childEnd = childStart + childSize;
                const childEarlyHalfWayPoint = childStart + (childSize * 0.45);
                const childLateHalfWayPoint = childStart + (childSize * 0.55);

                return {
                    childStart,
                    childSize,
                    childEnd,
                    childEarlyHalfWayPoint,
                    childLateHalfWayPoint,
                }
            },
        }
    });
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;

    .slide-drawer {
        height: 340px;
        background-color: white;
        position: relative;

        &.slide-drawer-closed {
            .slide-drawer-items {
                display: none;
            }

            .slide-drawer-controls {
                display: none;
            }

            height: auto;
        }

        .slide-drawer-items {
            position: absolute;
            top: 60px;
            left: 0;
            right: 0;
            bottom: 0;
            margin: 0;
            padding: 10px;
            overflow-x: scroll;
            display: flex;
            align-items: flex-start;
        }

        .slide-drawer-opener {
            display: none;
        }

        .slide-drawer-item {
            list-style-type: none;
            margin: 0;
            padding: 10px;
            width: 220px;
            flex: none;

            button {
                width: 100%;
                height: 100%;
                min-height: 140px;
                background-color: transparent;
                border: 3px solid transparent;
                border-radius: 15px;
                display: flex;
                flex-direction: column;
                align-items: center;
                font-size: 16px;
                font-family: $font-stack-bold;
                color: $nhsuk-blue;
                text-decoration: underline;
                padding: 15px 5px 5px 5px;

                picture {
                    flex-shrink: 0;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    width: 100px;
                    height: 100px;
                    border: 1px solid $nhsuk-grey-placeholder;
                    border-radius: 30%;
                    background-color: $nhsuk-grey-lighter;
                    overflow: hidden;

                    img {
                        width: 100%;
                        height: 100%;
                        object-fit: contain;
                        object-position: center center;
                    }
                }

                span {
                    margin-top: 10px;
                    word-break: break-word;
                }

                &:focus {
                    border-color: $nhsuk-white;
                    background-color: $govuk-focus-highlight-yellow;
                    border-bottom: 3px solid black;
                    color: $nhsuk-black;
                }
            }

            &-selected {
                button {
                    border-color: $nhsuk-green;

                    &:focus {
                        border: 3px solid $nhsuk-green;
                        box-shadow: none;
                    }
                }
            }

            .slide-drawer-item-just-selected:focus {
                background-color: transparent;
                color: $nhsuk-blue;
            }
        }

        .slide-drawer-controls {
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

        .slide-drawer-button {
            pointer-events: all;
        }

        &.slide-drawer-fullscreen {
            height: 100vh;
            display: flex;
            flex-direction: column;
            border: none;
            border-radius: 0;

            .slide-drawer-opener {
                position: absolute;
                display: block;
            }

            .slide-drawer-controls {
                display: none;
            }

            @media screen and (orientation: landscape) {
                height: auto;
                width: 250px;
                transition: width 1s;

                .slide-drawer-items {
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

                .slide-drawer-opener {
                    top: 20px;
                    right: -375px;
                    width: max-content;
                }

                &.slide-drawer-closed {
                    width: 0;
                }
            }

            @media screen and (orientation: portrait) {
                height: 250px;
                width: auto;
                transition: height 1s;

                .slide-drawer-opener {
                    top: -60px;
                    left: 20px;
                }

                &.slide-drawer-closed {
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
            padding: 19px 19px;
            height: 98px;
            width: 98px;
        }
    }
</style>
